namespace MakerFlightRC.Runtime.Input
{
    public interface IInputProvider
    {
        InputState CurrentState { get; }
    }
}
