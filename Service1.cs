using GerenciadorEmail.Models;
using OpenPop.Mime;
using OpenPop.Pop3;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GerenciadorEmail
{
    public partial class Service1 : ServiceBase
    {
        private Timer _timer;

        // dados para conexão 
        private string hostname = "mail.grieg.com.br";
        private int port = 110;
        private bool useSsl = false;
        private string username = "helpdesk";
        private string password = "************";

        public Service1()
        {
            InitializeComponent();
        }
        // quando iniciar o app 
        protected override void OnStart(string[] args)
        {
           _timer = new Timer(CriarLog, null, 0, 120000);

        }
        // quando fechar o app
        protected override void OnStop()
        {
        }

        private void CriarLog(object state )
        {
            List<Email> PegarMensagens = GetMensagens();

            if (PegarMensagens.Count > 0)
            {
                foreach (var email in PegarMensagens)
                {
                    StreamWriter vWriter = new StreamWriter(@"C:\Logs\log.txt", true);
                    vWriter.WriteLine("Servico Email ONLINE " + DateTime.Now + "Assunto =" + email.Assunto.ToString() +  "Corpo Mensagem ="+ email.ConteudoTexto);
                    vWriter.Flush();
                    vWriter.Close();
                }

            }
        }

     // pegar as mensaguens
        public List<Email> GetMensagens()
        {
            // The client disconnects from the server when being disposed
            using (Pop3Client client = new Pop3Client())
            {
                // Connect to the server
                client.Connect(hostname, port, useSsl);

                // Authenticate ourselves towards the server
                client.Authenticate(username, password);

                // Get the number of messages in the inbox
                int messageCount = client.GetMessageCount();

                // We want to download all messages
                List<Message> allMessages = new List<Message>(messageCount);
                List<Email> ListEmails = new List<Email>();


                // Messages are numbered in the interval: [1, messageCount]
                // Ergo: message numbers are 1-based.
                // Most servers give the latest message the highest number

                // adicionando mensagens na classe allMessages
                for (int i = messageCount; i > 0; i--)
                {
                    allMessages.Add(client.GetMessage(i));
                }
                // Numero da mensagem server para identificar a mensagem que sera apagada.

                // verificando se tem emails 
                foreach (var message in allMessages)
                {
                    List<Anexo> anexos = new List<Anexo>();
                    var popText = message.FindFirstPlainTextVersion();
                    var popHtml = message.FindFirstHtmlVersion();

                    string mailText = string.Empty;
                    string mailHtml = string.Empty;


                    if (popText != null)
                        mailText = popText.GetBodyAsText();
                    if (popHtml != null)
                        mailHtml = popHtml.GetBodyAsText();

                    // verificando se possue anexos e adicionando na classe ANEXO
                    if (message.MessagePart.MessageParts[1].IsAttachment == true)
                    {
                        foreach (MessagePart attachment in message.FindAllAttachments())
                        {
                            anexos.Add(new Anexo
                            {
                                FileByte = attachment.Body,
                                FileName = attachment.FileName,
                                FileType = attachment.ContentType.MediaType
                            });
                        }
                    }

                    //adicionando os emails na classe desiginada
                    ListEmails.Add(new Email()
                    {
                        Id = message.Headers.MessageId,
                        Assunto = message.Headers.Subject,
                        De = message.Headers.From.Address,
                        Para = string.Join("; ", message.Headers.To.Select(to => to.Address)),
                        Data = message.Headers.DateSent,
                        ConteudoTexto = mailText,
                        ConteudoHtml = !string.IsNullOrWhiteSpace(mailHtml) ? mailHtml : mailText,
                        Anexos = anexos

                    });

                    //apagar mensagens por id 
                    //if (message.Headers.Subject == "teste 6")
                    //{
                    //    var teste = DeleteMessageByMessageId(message.Headers.MessageId);
                    //}

                }
                return ListEmails;
            }
        }
    }
}
