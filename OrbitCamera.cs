using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Egc;

public class OrbitCamera : Camera
{
    private static readonly Vector3 StartTarget = new(0.0f, 1.0f, 0.0f);

    private Vector3 _target;
    private float _distance;
    private float _yaw;
    private float _pitch;

    public float MouseSensitivity = 0.25f;
    public float ZoomStep = 0.8f;
    public float MinDistance = 2.0f;
    public float MaxDistance = 40.0f;
    public float MoveSpeed = 3.0f;

    public override string Name => "Orbită";

    public override Vector3 Position
    {
        get
        {
            Vector3 direction = GetDirectionFromTargetToCamera();
            return _target + direction * _distance;
        }
    }

    public override Vector3 Front =>
        Vector3.Normalize(_target - Position);

    public override Matrix4 ViewMatrix =>
        Matrix4.LookAt(Position, _target, Vector3.UnitY);

    public OrbitCamera()
    {
        Reset();
    }

    public override void Update(
        KeyboardState keyboard,
        MouseState mouse,
        Vector2 mouseDelta,
        float dt)
    {
        if (mouse.IsButtonDown(MouseButton.Left))
        {
            _yaw += mouseDelta.X * MouseSensitivity;
            _pitch -= mouseDelta.Y * MouseSensitivity;
            _pitch = Math.Clamp(_pitch, -89.0f, 89.0f);
        }

        if (mouse.ScrollDelta.Y > 0.0f)
        {
            _distance -= ZoomStep;
        }
        else if (mouse.ScrollDelta.Y < 0.0f)
        {
            _distance += ZoomStep;
        }

        _distance = Math.Clamp(_distance, MinDistance, MaxDistance);

        Vector3 forward = new Vector3(Front.X, 0.0f, Front.Z);
        Vector3 right = Vector3.Cross(forward, Vector3.UnitY);
        Vector3 movement = Vector3.Zero;

        if (forward.LengthSquared > 0.0f)
        {
            forward = Vector3.Normalize(forward);
            right = Vector3.Normalize(right);

            if (keyboard.IsKeyDown(Keys.W)) movement += forward;
            if (keyboard.IsKeyDown(Keys.S)) movement -= forward;
            if (keyboard.IsKeyDown(Keys.D)) movement += right;
            if (keyboard.IsKeyDown(Keys.A)) movement -= right;
        }

        if (movement.LengthSquared > 0.0f)
        {
            movement = Vector3.Normalize(movement);
            float speed = keyboard.IsKeyDown(Keys.LeftShift)
                ? MoveSpeed * 2.5f
                : MoveSpeed;
            _target += movement * speed * dt;
        }
    }

    private Vector3 GetDirectionFromTargetToCamera()
    {
        float yawRadians = MathHelper.DegreesToRadians(_yaw);
        float pitchRadians = MathHelper.DegreesToRadians(_pitch);

        return new Vector3(
            MathF.Cos(yawRadians) * MathF.Cos(pitchRadians),
            MathF.Sin(pitchRadians),
            MathF.Sin(yawRadians) * MathF.Cos(pitchRadians));
    }

    public override void Reset()
    {
        _target = StartTarget;
        _distance = 10.0f;
        _yaw = 90.0f;
        _pitch = 20.0f;
        FieldOfView = 60.0f;
    }

    public void SetFromFps(FpsCamera fps)
    {
        _distance = 5.0f;
        _target = fps.Position + fps.Front * _distance;

        Vector3 direction = -fps.Front;
        _yaw = MathHelper.RadiansToDegrees(MathF.Atan2(direction.Z, direction.X));
        _pitch = MathHelper.RadiansToDegrees(MathF.Asin(direction.Y));
        FieldOfView = fps.FieldOfView;
    }

    public override IEnumerable<string> Describe()
    {
        Vector3 position = Position;

        yield return $"Poziție        : ({position.X,6:F2}, {position.Y,6:F2}, {position.Z,6:F2})";
        yield return $"Țintă          : ({_target.X,6:F2}, {_target.Y,6:F2}, {_target.Z,6:F2})";
        yield return $"Distanță       : {_distance,6:F2}";
        yield return $"Azimut         : {_yaw,6:F1}°";
        yield return $"Elevație       : {_pitch,6:F1}°";
        yield return $"Front          : ({Front.X,6:F2}, {Front.Y,6:F2}, {Front.Z,6:F2})";
    }
}