using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatrimonioBusiness.bases
{
    internal abstract class BaseRepositorio<T> : IDisposable where T : class
    {
        private DbContext contexto = null;
        protected StringBuilder builderException = null;
        protected IQueryable<T> createIQueryable = null;
        protected IsolationLevel isolationLevel = IsolationLevel.ReadUncommitted;
        protected BaseRepositorio(IsolationLevel isolationLevel, DbContext contexto)
        {
            this.isolationLevel = isolationLevel;
            this.contexto = contexto;
        }
       
        protected virtual DbContext GetContexto()
        {
            return this.contexto;
        }
        protected void ConfiguraContexto()
        {
            this.DesabiitarLazyLoading();
            this.DesabilitarProxy();
            this.contexto.Configuration.AutoDetectChangesEnabled = false;
        }
        internal DbContextTransaction createTransacao()
        {
            if (this.contexto.Database.CurrentTransaction != null)
                this.contexto.Database.CurrentTransaction.Commit();

            return this.contexto.Database.BeginTransaction(this.isolationLevel);
        }
        internal void useTransacao(DbTransaction transacao)
        {
            this.contexto.Database.UseTransaction(transacao);

        }
        internal void Commit()
        {
            this.contexto.Database.CurrentTransaction.Commit();
        }
        internal void Rollback()
        {
            this.contexto.Database.CurrentTransaction.Rollback();
        }
        internal void DesabiitarLazyLoading()
        {
            this.contexto.Configuration.LazyLoadingEnabled = false;
        }
        internal void HabilitarazyLoading()
        {
            this.contexto.Configuration.LazyLoadingEnabled = true;
        }
        internal void habilitarProxy()
        {
            this.contexto.Configuration.ProxyCreationEnabled = true;
        }
        internal void DesabilitarProxy()
        {
            this.contexto.Configuration.ProxyCreationEnabled = false;
        }
        public void Dispose()
        {
            this.contexto.Dispose();
        }
        internal virtual void Adicionar(T obj)
        {
            this.contexto.Set<T>().Add(obj);
        }
        internal virtual void Adicionar(IList<T> objs)
        {
            this.contexto.Set<T>().AddRange(objs);
        }
        internal virtual void Atualizar(T obj)
        {
            this.contexto.Entry(obj).State = EntityState.Modified;

        }
        internal virtual void Atualizar(IList<T> objs)
        {
            this.contexto.Entry(objs).State = EntityState.Modified;

        }
        internal void Excluir(Func<T, bool> predicate)
        {
            this.contexto.Set<T>().Where(predicate).AsQueryable().ToList().ForEach(d => this.contexto.Set<T>().Remove(d));

        }

        internal T Find(params object[] key)
        {
            return this.contexto.Set<T>().Find(key);
        }

        internal int Salvar()
        {
            try
            {
                return this.contexto.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entidade do tipo \"{0}\" no estado \"{1}\" tem os seguintes erros de validação:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Erro: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }

            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
               

            }

        }
        internal Task<int> ExecuteNoQuery(string query)
        {
            this.contexto.Database.CommandTimeout = 0;
            this.contexto.Configuration.AutoDetectChangesEnabled = false;
            this.contexto.Configuration.ValidateOnSaveEnabled = false;
            return this.contexto.Database.ExecuteSqlCommandAsync(query, TransactionalBehavior.DoNotEnsureTransaction);

        }
        internal Task<int> SalvarAnsyc()
        {
            try
            {
                this.contexto.Configuration.AutoDetectChangesEnabled = false;
                this.contexto.Configuration.ValidateOnSaveEnabled = false;
                this.contexto.Database.CommandTimeout = 0;
                return this.contexto.SaveChangesAsync();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entidade do tipo \"{0}\" no estado \"{1}\" tem os seguintes erros de validação:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Erro: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }

            catch (SqlException ex)
            {
               throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
              

            }

        }

    }
}
