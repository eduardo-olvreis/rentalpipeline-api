namespace RentalPipeline.Exceptions
{
    public class ConflitoConcorrenciaException : Exception
    {
        public ConflitoConcorrenciaException(string message) : base(message) { }
    }
}
