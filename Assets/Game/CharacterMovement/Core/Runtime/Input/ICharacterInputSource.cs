namespace UnityGame.CharacterMovement
{
    public interface ICharacterInputSource
    {
        CharacterInputFrame CurrentFrame { get; }

        CharacterInputFrame ReadInput();
    }
}
