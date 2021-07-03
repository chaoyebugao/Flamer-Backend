using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;
using System;
using System.Threading.Tasks;

namespace Flammer.Service.Email
{
    public interface IEmailService
    {
        /// <summary>
        /// 发送注册激活邮件
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="targetAddress">接收地址</param>
        /// <param name="activationUrl">激活链接地址</param>
        Task SendSignUpActivationAsync(string userName, string targetAddress, string activationUrl);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration configuration;

        public EmailService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        /// <summary>
        /// 发送注册激活邮件
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="targetAddress">接收地址</param>
        /// <param name="activationUrl">激活链接地址</param>
        public Task SendSignUpActivationAsync(string userName, string targetAddress, string activationUrl)
        {
            return Task.Run(() =>
            {
                var subject = "Flamer注册激活";
                var bodyText = $"<p>您已完成注册，请及时点击链接激活账户后登录: <a href=\"{activationUrl}\">{activationUrl}</a></p>";

                var host = configuration["Email:Host"];
                var port = int.Parse(configuration["Email:Port"]);
                var username = configuration["Email:Username"];
                var password = configuration["Email:Password"];

                var message = new MimeMessage();
                //发件人
                message.From.Add(new MailboxAddress("Flamer", username));
                //收件人
                message.To.Add(new MailboxAddress(userName, targetAddress));
                //标题
                message.Subject = subject;
                //产生一个支持HTml 的TextPart
                var body = new TextPart(TextFormat.Html)
                {
                    Text = bodyText,
                };

                //创建Multipart添加附件
                var multipart = new Multipart("mixed")
                {
                    body,
                };
                //正文内容，发送
                message.Body = multipart;
                using var client = new MailKit.Net.Smtp.SmtpClient();
                client.CheckCertificateRevocation = false;
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                //Smtp服务器
                client.Connect(host, port, true);

                //登录，发送
                //特别说明，对于服务器端的中文相应，Exception中有编码问题，显示乱码了
                client.Authenticate(username, password);//授权码
                client.Send(message);
                //断开
                client.Disconnect(true);
                Console.WriteLine("发送邮件成功");

            });
        }
    }
}
