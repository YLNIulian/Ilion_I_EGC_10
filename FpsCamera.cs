using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Egc;

public class FpsCamera : Camera
{
    private Vector3 _position;

    public override Vector3 Position => _position;

    public float Yaw { get; private set; }
    public float Pitch { get; private set; }

    public bool FreeFly { get; set; }

    private Vector3 _front = -Vector3.UnitZ;
    private Vector3 _right = Vector3.UnitX;
    private Vector3 _up = Vector3.UnitY;

    public override Vector3 Front => _front;

    public const float MaxPitch = 89.0f;
    public float MoveSpeed = 3.0f;
    public float SprintFactor = 2.5f;
    public float MouseSensitivity = 0.1f;
    public float FovStep = 2.0f;

    private static readonly Vector3 WorldUp = Vector3.UnitY;
    private static readonly Vector3 StartPosition = new(0.0f, 1.7f, 6.0f);

    private const float StartYaw = -90.0f;
    private const float StartPitch = 0.0f;

    public FpsCamera()
    {
        Reset();
    }

    public override string Name =>
        FreeFly ? "FPS - zbor liber" : "FPS - mers";

    public override Matrix4 ViewMatrix =>
        Matrix4.LookAt(_position, _position + _front, _up);

    public override void Update(
        KeyboardState keyboard,
        MouseState mouse,
        Vector2 mouseDelta,
        float dt)
    {
        Yaw += mouseDelta.X * MouseSensitivity;
        Pitch -= mouseDelta.Y * MouseSensitivity;
        Pitch = Math.Clamp(Pitch, -MaxPitch, MaxPitch);

        UpdateVectors();

        if (mouse.ScrollDelta.Y > 0.0f)
        {
            ChangeFov(-FovStep);
        }
        else if (mouse.ScrollDelta.Y < 0.0f)
        {
            ChangeFov(FovStep);
        }

        Vector3 forward;

        if (FreeFly)
        {
            forward = _front;
        }
        else
        {
            forward = new Vector3(_front.X, 0.0f, _front.Z);

            if (forward.LengthSquared > 0.0f)
            {
                forward = Vector3.Normalize(forward);
            }
        }

        Vector3 movement = Vector3.Zero;

        if (keyboard.IsKeyDown(Keys.W))
        {
            movement += forward;
        }

        if (keyboard.IsKeyDown(Keys.S))
        {
            movement -= forward;
        }

        if (keyboard.IsKeyDown(Keys.D))
        {
            movement += _right;
        }

        if (keyboard.IsKeyDown(Keys.A))
        {
            movement -= _right;
        }

        if (FreeFly && keyboard.IsKeyDown(Keys.Space))
        {
            movement += WorldUp;
        }

        if (FreeFly && keyboard.IsKeyDown(Keys.LeftControl))
        {
            movement -= WorldUp;
        }

        if (movement.LengthSquared > 0.0f)
        {
            movement = Vector3.Normalize(movement);

            float speed = MoveSpeed;

            if (keyboard.IsKeyDown(Keys.LeftShift))
            {
                speed *= SprintFactor;
            }

            _position += movement * speed * dt;
        }
    }

    private void UpdateVectors()
    {
        float yawRadians = MathHelper.DegreesToRadians(Yaw);
        float pitchRadians = MathHelper.DegreesToRadians(Pitch);

        _front = new Vector3(
            MathF.Cos(yawRadians) * MathF.Cos(pitchRadians),
            MathF.Sin(pitchRadians),
            MathF.Sin(yawRadians) * MathF.Cos(pitchRadians));

        _front = Vector3.Normalize(_front);
        _right = Vector3.Normalize(Vector3.Cross(_front, WorldUp));
        _up = Vector3.Normalize(Vector3.Cross(_right, _front));
    }

    public override void Reset()
    {
        _position = StartPosition;
        Yaw = StartYaw;
        Pitch = StartPitch;
        FieldOfView = 60.0f;
        FreeFly = false;

        UpdateVectors();
    }

    public void SetPose(Vector3 position, Vector3 front)
    {
        _position = position;
        _front = Vector3.Normalize(front);
        Pitch = MathHelper.RadiansToDegrees(MathF.Asin(_front.Y));
        Yaw = MathHelper.RadiansToDegrees(MathF.Atan2(_front.Z, _front.X));
        UpdateVectors();
    }

    public override IEnumerable<string> Describe()
    {
        yield return $"Poziție        : ({_position.X,6:F2}, {_position.Y,6:F2}, {_position.Z,6:F2})";
        yield return $"Yaw / pitch    : {NormalizeAngle(Yaw),7:F1}° / {Pitch,6:F1}°";
        yield return $"Front          : ({_front.X,6:F2}, {_front.Y,6:F2}, {_front.Z,6:F2})";
        yield return $"Right          : ({_right.X,6:F2}, {_right.Y,6:F2}, {_right.Z,6:F2})";
        yield return $"Up             : ({_up.X,6:F2}, {_up.Y,6:F2}, {_up.Z,6:F2})";
    }

    private static float NormalizeAngle(float degrees)
    {
        degrees %= 360.0f;

        if (degrees > 180.0f)
        {
            degrees -= 360.0f;
        }

        if (degrees <= -180.0f)
        {
            degrees += 360.0f;
        }

        return degrees;
    }
}
