namespace Notino.Common.Service
{
    public interface IMailService
    {
        void SendMail(string emailAddress, byte[] fileAttachment, string fileName);
    }
}
