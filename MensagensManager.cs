using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using Entities.Models;
using Entities.Services;
using Libs;

namespace Manager
{
    public class MensagensManager
    {
        public static void ProcessarContatos(Mensagens mensagem, ContatosMensagens contatoMensagem)
        {
            Contatos contato = contatoMensagem.Contato;

            string codigo = String.Empty;
            if (mensagem.Tipo > 1)
            {
                codigo = AppSystem.GerarCodigoRandomico();
                GetValue(mensagem, contato, codigo);
            }
            
            try
            {
                string sms = MountSms(mensagem, contato, codigo);

                GSMMannager gsm = new GSMMannager();
                gsm.EnviarMensagem(sms, AppSystem.TratarTelefone(contato.Telefone));

                ContatosMensagensService cmService = new ContatosMensagensService();
                MensagensDisparosService mdService = new MensagensDisparosService();
                MensagensDisparos md = new MensagensDisparos();
                cmService.CheckUpdate(contato.Id, mensagem.Id);

                md.SetParams(mensagem.Id, contato.Id, mensagem.Mensagem.Length, DateTime.Now, 1);
                mdService.Add(md);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static void GetValue(Mensagens mensagem, Contatos contato, string codigo)
        {
            PromocoesService service = new PromocoesService();
            ICollection<Promocoes> list = service.Consult(mensagem.Id.ToString(), "Mensagem");
            int checkPromocoes = list.Count;
            if (String.IsNullOrEmpty(codigo) || checkPromocoes == 0)
            {
                return;
            }
            Promocoes promocao = list.First();

            CodigosPromocionaisService cpService = new CodigosPromocionaisService();
            CodigosPromocionais cp = new CodigosPromocionais();

            cp.SetParams(promocao.Id, contato.Id, codigo, 1);
            cp.Created = DateTime.Now;

            cpService.Add(cp);
        }

        private static string MountSms(Mensagens mensagem, Contatos contato, string codigo)
        {
            string sms;
            switch (mensagem.Tipo)
            {
                case 1:
                    sms = GetFirstName(contato.Nome) + " " + mensagem.Mensagem;
                    break;
                case 2:
                    sms = mensagem.Mensagem + " " + codigo;
                    break;
                case 3:
                    sms = GetFirstName(contato.Nome) + " " + mensagem.Mensagem + " " + codigo;
                    break;
                default:
                    sms = mensagem.Mensagem;
                    break;
            }
            return Validacao.TratarMensagem(sms);
        }

        public static bool ProcessarMensagem(int id)
        {
            if (AppConfig.GetValue("porta") == "N")
            {
                throw new Exception(
                    "Moldem GSM não localizado. Verifique se o dispositivo está conectado e tente novamente");
            }

            MensagensService mService = new MensagensService();
            Mensagens mensagem = mService.Find(id);

            ICollection<ContatosMensagens> cMensagens = mensagem.ContatosMensagens;
            int count = cMensagens.Count;
            if (count == 0)
            {
                return true;
            }

            foreach (ContatosMensagens item in cMensagens)
            {
                if (item.Status == 1)
                {
                    ProcessarContatos(mensagem, item);
                }
            }
            mensagem.Status = 2;
            mService.Edit(mensagem);

            ContatosMensagensService cmService = new ContatosMensagensService();
            cmService.ReorganizarContatosMensagens(id);

            return true;
        }

        public static void DispararMensagem()
        {
            AgendasMensagensService amService = new AgendasMensagensService();

            // Verificando se o agendamento dos aniversariantes existe
            ICollection<AgendasMensagens> checarAniversariantes = amService.ConsultByMensagemByEnvio(1, DateTime.Today);
            int count = checarAniversariantes.Count;
            if (count == 0)
            {
                // Agendando mensagens
                AgendarAniversariantes();
            }

            // Buscando todos os agendamentos para a data
            ICollection<AgendasMensagens> agendamentos = amService.Consult(DateTime.Today.ToString(), "DataEnvio");
            foreach (AgendasMensagens item in agendamentos)
            {
                ProcessarMensagem(item.MensagemId);

                item.Status = 2;

                amService.Edit(item);
            }
        }

        public static void AgendarAniversariantes()
        {
            // Limpando todas as ligações da mensagem de aniversário
            ContatosMensagensService cmService = new ContatosMensagensService();
            ICollection<ContatosMensagens> remover = cmService.Consult("1", "Mensagem");
            int count = remover.Count;
            if (count > 0)
            {
                foreach (ContatosMensagens item in remover)
                {
                    cmService.Remove(item);
                }
            }

            // Carregando mensagem de aniversário
            MensagensService mService = new MensagensService();
            Mensagens mensagem = mService.Find(1);

            // Buscando contatos para verificar aniversário
            ContatosService cService = new ContatosService();
            ICollection<Contatos> contatos = cService.Consult("status", null);

            // Amarrando contatos a nova mensagem
            foreach (Contatos item in contatos)
            {
                if (item.DataNascimento.HasValue)
                {
                    DateTime dataAniversario = item.DataNascimento.Value;

                    int dia = dataAniversario.Day;
                    int mes = dataAniversario.Month;

                    DateTime dataAtual = DateTime.Today;
                    // verificando se o contato é aniversariante
                    if (dia == dataAtual.Day && mes == dataAtual.Month)
                    {
                        ContatosMensagens cm = new ContatosMensagens();
                        cm.SetParams(mensagem.Id, item.Id, 1);
                        cmService.Add(cm);
                    }
                }
            }
            AgendasMensagensService amService = new AgendasMensagensService();
            AgendasMensagens am = new AgendasMensagens();

            am.SetParams(mensagem.Id, DateTime.Today, 1);
            amService.Add(am);
        }

        public static string GetFirstName(string parametro)
        {
            string[] nome = parametro.Substring(0).Split(' ');

            return nome[0];
        }
    }
}