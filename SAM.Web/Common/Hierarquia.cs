using Sam.Common.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SAM.Web.Models;
using SAM.Web.Context;

namespace SAM.Web.Common
{
    public class Hierarquia
    {
        private HierarquiaContext dbHierarquia = new HierarquiaContext();

        public List<Profile> GetProfiles(bool? flagPerfilMaster)
        {
            List<Profile> query;
            using (dbHierarquia = new HierarquiaContext()) {

                if (flagPerfilMaster == true)
                {
                    query = (from q in this.dbHierarquia.Profiles
                             where q.Status == true
                             select q).ToList();
                }
                else
                {
                    query = (from q in this.dbHierarquia.Profiles
                             where q.flagPerfilMaster != true
                             && q.Status == true
                             select q).ToList();
                }
           }

            Profile profile = new Profile();
            profile.Id = 0;
            profile.Description = "Selecione o Perfil";
            query.Add(profile);

            return query.OrderBy(p => p.Id).ToList();
        }
        public List<Institution> GetOrgaos(int? _institutionId)
        {
            
            List<Institution> query;
            using (dbHierarquia = new HierarquiaContext()) {

                if (_institutionId.HasValue && _institutionId != 0)
                {

                    query = (from og in this.dbHierarquia.Institutions
                             where og.Status == true && og.Id == _institutionId
                             select og).ToList();

                    query.ForEach(o => o.Description = o.Code + " - " + o.Description);
                }
                else
                {
                    query = (from og in this.dbHierarquia.Institutions
                             where og.Status == true
                             select og).ToList();

                    query.ForEach(o => o.Description = o.Code + " - " + o.Description);

                    Institution institution = new Institution();
                    institution.Id = 0;
                    institution.Code = "0";
                    institution.Description = "Selecione o Orgão";
                    query.Add(institution);
                }
            }

            return query.ToList().OrderBy(o => int.Parse(o.Code)).ToList();
        }

        public List<Institution> GetOrgaosMesmaGestao(string _managerCode, int? _institutionId)
        {
            List<Institution> query;
            using (dbHierarquia = new HierarquiaContext()) {
                if (_institutionId.HasValue && _institutionId != 0)
                {

                    query = (from og in this.dbHierarquia.Institutions
                             where og.Status == true &&
                                   og.Id == _institutionId &&
                                   og.ManagerCode == _managerCode

                             select og).ToList();

                    query.ForEach(o => o.Description = o.Code + " - " + o.Description);
                }
                else
                {
                    query = (from og in this.dbHierarquia.Institutions
                             where og.Status == true &&
                                   og.ManagerCode == _managerCode

                             select og).ToList();

                    query.ForEach(o => o.Description = o.Code + " - " + o.Description);

                    Institution institution = new Institution();
                    institution.Id = 0;
                    institution.Code = "0";
                    institution.Description = "Selecione o Orgão";
                    query.Add(institution);
                }
            }

            return query.ToList().OrderBy(o => int.Parse(o.Code)).ToList();
        }

        public List<Institution> GetOrgaosMesmaGestaoNaoImplantados(string _managerCode)
        {
            List<Institution> query;
            using (dbHierarquia = new HierarquiaContext())
            {
                    query = (from og in this.dbHierarquia.Institutions
                             where og.Status == true &&
                                   og.ManagerCode == _managerCode &&
                                   !og.flagImplantado
                             select og).ToList();

                    query.ForEach(o => o.Description = o.Code + " - " + o.Description);

                    Institution institution = new Institution();
                    institution.Id = 0;
                    institution.Code = "0";
                    institution.Description = "Selecione o Orgão";
                    query.Add(institution);
                
            }

            return query.ToList().OrderBy(o => int.Parse(o.Code)).ToList();
        }

        public List<Institution> GetOrgaosOutraGestaoNaoImplantados(string _managerCode)
        {
            List<Institution> query;
            using (dbHierarquia = new HierarquiaContext())
            {
                query = (from og in this.dbHierarquia.Institutions
                         where og.Status == true &&
                               og.ManagerCode != _managerCode &&
                               !og.flagImplantado
                         select og).ToList();

                query.ForEach(o => o.Description = o.Code + " - " + o.Description);

                Institution institution = new Institution();
                institution.Id = 0;
                institution.Code = "0";
                institution.Description = "Selecione o Orgão";
                query.Add(institution);

            }

            return query.ToList().OrderBy(o => int.Parse(o.Code)).ToList();
        }

        public List<Institution> GetOrgaosGestaoDiferente(string _managerCode, int? _institutionId)
        {
            List<Institution> query;

            using (dbHierarquia = new HierarquiaContext()) {
                if (_institutionId.HasValue && _institutionId != 0)
                {

                    query = (from og in this.dbHierarquia.Institutions
                             where og.Status == true &&
                                   og.Id == _institutionId &&
                                   og.ManagerCode != _managerCode

                             select og).ToList();

                    query.ForEach(o => o.Description = o.Code + " - " + o.Description);
                }
                else
                {
                    query = (from og in this.dbHierarquia.Institutions
                             where og.Status == true &&
                                   og.ManagerCode != _managerCode

                             select og).ToList();

                    query.ForEach(o => o.Description = o.Code + " - " + o.Description);

                    Institution institution = new Institution();
                    institution.Id = 0;
                    institution.Code = "0";
                    institution.Description = "Selecione o Orgão";
                    query.Add(institution);
                }
            }

            return query.ToList().OrderBy(o => int.Parse(o.Code)).ToList();
        }

        public List<BudgetUnit> GetUosPorOrgaoId(int _institutionId)
        {
            List<BudgetUnit> query;

            using (dbHierarquia = new HierarquiaContext())
            {
                query = (from b in this.dbHierarquia.BudgetUnits
                         where b.InstitutionId == _institutionId
                            && b.Status == true
                         select b).ToList();
            }

            query.ForEach(o => o.Description = o.Code + " - " + o.Description);

            BudgetUnit budgetUnit = new BudgetUnit();
            budgetUnit.Id = 0;
            budgetUnit.Code = "0";
            budgetUnit.Description = "Selecione a UO";

            query.Add(budgetUnit);

            return query.ToList().OrderBy(u => int.Parse(u.Code)).ToList();
        }
        public List<BudgetUnit> GetUos(int? _budgetUnitId)
        {
            List<BudgetUnit> query;

            if (_budgetUnitId.HasValue && _budgetUnitId != 0)
            {
                using (dbHierarquia = new HierarquiaContext())
                {
                    query = (from b in this.dbHierarquia.BudgetUnits
                             where b.Id == _budgetUnitId && b.Status == true
                             select b).ToList();

                    query.ForEach(o => o.Description = o.Code + " - " + o.Description);
                }
            }
            else
            {
                query = new List<BudgetUnit>();

                BudgetUnit budgetUnit = new BudgetUnit();
                budgetUnit.Id = 0;
                budgetUnit.Code = "0";
                budgetUnit.Description = "Selecione a UO";
                query.Add(budgetUnit);
            }

            return query.ToList().OrderBy(u => int.Parse(u.Code)).ToList();
        }
        public List<ManagerUnit> GetUgesPorUoId(int? _budgetUnitId)
        {
            List<ManagerUnit> query;

            if (_budgetUnitId.HasValue && _budgetUnitId != 0)
            {
                using (dbHierarquia = new HierarquiaContext())
                {
                    query = (from m in this.dbHierarquia.ManagerUnits
                             where m.BudgetUnitId == _budgetUnitId && m.Status == true
                             select m).ToList();
                }
                query.ForEach(o => o.Description = o.Code + " - " + o.Description);

                ManagerUnit managerUnit = new ManagerUnit();
                managerUnit.Id = 0;
                managerUnit.Code = "0";
                managerUnit.Description = "Selecione a UGE";
                query.Add(managerUnit);
            }
            else
            {
                query = new List<ManagerUnit>();
                ManagerUnit managerUnit = new ManagerUnit();

                managerUnit.Id = 0;
                managerUnit.Code = "0";
                managerUnit.Description = "Selecione a UGE";
                query.Add(managerUnit);
            }

            return query.ToList().OrderBy(uge => int.Parse(uge.Code)).ToList();
        }
        public List<ManagerUnit> GetUges(int? _managerUnitId)
        {
            List<ManagerUnit> query;

            if (_managerUnitId.HasValue && _managerUnitId != 0)
            {
                using (dbHierarquia = new HierarquiaContext())
                {
                    query = (from m in this.dbHierarquia.ManagerUnits
                             where m.Id == _managerUnitId && m.Status == true
                             select m).ToList();
                }
                query.ForEach(o => o.Description = o.Code + " - " + o.Description);
            }
            else
            {
                query = new List<ManagerUnit>();
                ManagerUnit managerUnit = new ManagerUnit();

                managerUnit.Id = 0;
                managerUnit.Code = "0";
                managerUnit.Description = "Selecione a UGE";
                query.Add(managerUnit);
            }

            return query.ToList().OrderBy(uge => int.Parse(uge.Code)).ToList();
        }
        public List<AdministrativeUnit> GetUasPorUgeId(int? _managerUnitId)
        {
            List<AdministrativeUnit> query;

            if (_managerUnitId.HasValue && _managerUnitId != 0)
            {
                using (dbHierarquia = new HierarquiaContext())
                {
                    query = (from a in this.dbHierarquia.AdministrativeUnits
                             where a.ManagerUnitId == _managerUnitId
                                && a.Status == true
                             select a).ToList();
                }
                query.ForEach(o => o.Description = o.Code + " - " + o.Description);

                AdministrativeUnit administrativeUnit = new AdministrativeUnit();
                administrativeUnit.Id = 0;
                administrativeUnit.Code = 0;
                administrativeUnit.Description = "Selecione a UA";

                query.Add(administrativeUnit);
            }
            else
            {
                query = new List<AdministrativeUnit>();

                AdministrativeUnit administrativeUnit = new AdministrativeUnit();
                administrativeUnit.Id = 0;
                administrativeUnit.Code = 0;
                administrativeUnit.Description = "Selecione a UA";

                query.Add(administrativeUnit);
            }

            return query.ToList().OrderBy(ua => ua.Code).ToList();
        }
        public List<AdministrativeUnit> GetUas(int? _administrativeUnitId)
        {
            List<AdministrativeUnit> query;

            if (_administrativeUnitId.HasValue && _administrativeUnitId != 0)
            {
                using (dbHierarquia = new HierarquiaContext())
                {
                    query = (from a in dbHierarquia.AdministrativeUnits
                             where a.Id == _administrativeUnitId
                                && a.Status == true
                             select a).ToList();
                }

                query.ForEach(o => o.Description = o.Code + " - " + o.Description);
            }
            else
            {
                query = new List<AdministrativeUnit>();
                AdministrativeUnit administrativeUnit = new AdministrativeUnit();
                administrativeUnit.Id = 0;
                administrativeUnit.Code = 0;
                administrativeUnit.Description = "Selecione a UA";

                query.Add(administrativeUnit);
            }

            return query.ToList().OrderBy(ua => ua.Code).ToList();
        }
        public List<AdministrativeUnit> GetUasPorRelacaoCodigos(IEnumerable<int> relacaoAdministrativeUnitCodes)
        {
            IList<AdministrativeUnit> retornoListagemregistros;

            if (relacaoAdministrativeUnitCodes.HasElements())
            {
                using (dbHierarquia = new HierarquiaContext())
                {
                    retornoListagemregistros = dbHierarquia.AdministrativeUnits
                                                           .Where(uaSIAFEM => relacaoAdministrativeUnitCodes.Contains(uaSIAFEM.Code) && uaSIAFEM.Status == true)
                                                           .ToList();
                }
                retornoListagemregistros.ToList().ForEach(o => o.Description = o.Code + " - " + o.Description);
            }
            else
            {
                retornoListagemregistros = new List<AdministrativeUnit>();
                AdministrativeUnit administrativeUnit = new AdministrativeUnit();
                administrativeUnit.Id = 0;
                administrativeUnit.Code = 0;
                administrativeUnit.Description = "Selecione a(s) UA(s)";

                retornoListagemregistros.Add(administrativeUnit);
            }

            return retornoListagemregistros.ToList().OrderBy(ua => ua.Code).ToList();
        }
        public List<Section> GetDivisoes(int? _sectionId)
        {
            List<Section> query;

            if (_sectionId.HasValue && _sectionId != 0)
            {
                using (dbHierarquia = new HierarquiaContext())
                {
                    query = (from s in dbHierarquia.Sections
                             where s.Id == _sectionId && s.Status == true
                             select s).ToList();
                }

                query.ForEach(o => o.Description = o.Code + " - " + o.Description);
            }
            else
            {
                query = new List<Section>();
                Section section = new Section();
                section.Id = 0;
                section.Code = 0;
                section.Description = "Selecione a Divisão";

                query.Add(section);
            }

            return query.ToList().OrderBy(s => s.Code).ToList();
        }
        public List<Section> GetDivisoesPorUaId(int? _administrativeUnitId)
        {
            List<Section> query;

            if (_administrativeUnitId.HasValue && _administrativeUnitId != 0)
            {
                using (dbHierarquia = new HierarquiaContext())
                {
                    query = (from s in dbHierarquia.Sections
                             where s.AdministrativeUnitId == _administrativeUnitId && s.Status == true
                             select s).ToList();
                }

                query.ForEach(o => o.Description = o.Code + " - " + o.Description);

                Section section = new Section();
                section.Id = 0;
                section.Code = 0;
                section.Description = "Selecione a Divisão";

                query.Add(section);
            }
            else
            {
                query = new List<Section>();
                Section section = new Section();
                section.Id = 0;
                section.Code = 0;
                section.Description = "Selecione a Divisão";

                query.Add(section);
            }

            return query.ToList().OrderBy(s => s.Code).ToList();
        }

    }
}