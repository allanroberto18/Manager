using System;
using System.Linq;
using Entities.Models;
using Entities.Services;

namespace Manager
{
    public class ConfiguracoesManager
    {
        public static int CountAcessos()
        {
            UsuariosAcessosService service = new UsuariosAcessosService();

            return service.List().Count();
        }

        public static int CountConfiguracoes()
        {
            ConfiguracoesService service = new ConfiguracoesService();

            return service.List().Count();
        }

        public static void ProcessarConfiguracoes(int id)
        {
            int acessos = CountAcessos();
            int configuracoes = CountConfiguracoes();

            if (acessos > 0 || configuracoes > 0 || id == 1)
            {
                return;
            }

            ConfiguracoesService service = new ConfiguracoesService();
            service.Add(new Configuracoes() {Inicio = DateTime.Today, Fim = DateTime.Today.AddDays(5), Created = DateTime.Now, Status = 1});
        }

        public static int CheckLicenca()
        {
            ConfiguracoesService service = new ConfiguracoesService();
            Configuracoes entity = service.Find(1);

            return entity.Fim.CompareTo(DateTime.Today);
        }
    }
}