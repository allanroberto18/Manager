using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Entities.Models;
using Entities.Services;

namespace Manager
{
    public class LicencasManager
    {
        public static void Novo(string licenca)
        {
            LicencasService servico = new LicencasService();
            ICollection<Licencas> list = servico.Consult();
            int count = list.Count;
            
            Licencas entity;
            if (count > 0)
            {
                entity = servico.List().First(i => i.Licenca.Equals(licenca));
                entity.Licenca = licenca;
                entity.Updated = DateTime.Now;

                servico.Edit(entity);

                MessageBox.Show("Licença registrada com sucesso", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            entity = new Licencas();
            entity.SetParams(licenca, 1);
            entity.Created = DateTime.Now;

            servico.Add(entity);

            MessageBox.Show("Licença registrada com sucesso", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static string GetLicenca()
        {
            LicencasService servico = new LicencasService();
            ICollection<Licencas> check = servico.Consult();

            Licencas entity;
            int count = check.Count;
            if (count > 0)
            {
                entity = servico.List().First();
                return entity.Licenca;
            }
            entity = new Licencas();
            return entity.ToString();
        }

        public static void ValidarLicenca()
        {

        }
    }
}
