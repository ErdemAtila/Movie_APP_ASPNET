namespace CORE.APP.Models
{
    public class CommandResponse : Response // CommandResponse cr = new CommandResponse(true);
                                            // cr.Message = "False"; // can't be done
    {
        public bool IsSuccessful { get; }

        public string Message { get; }

        public CommandResponse(bool isSuccessful, string message = "", int id = 0) : base(id)
        {
            IsSuccessful = isSuccessful;
            Message = message;
        }
    }
}
