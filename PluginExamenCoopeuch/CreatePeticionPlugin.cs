using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;


//namespace
namespace PluginExamenCoopeuch
{
    public class CreatePeticionPlugin : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                Entity requerimiento = (Entity)context.InputParameters["Target"];
                try
                {
                    if (requerimiento.LogicalName == "incident")
                    { 
                        Entity peticion = new Entity("new_peticion");

                        //Campos solicitados con valores de Entidad Requerimiento
                        //Registro Petición relacionado con Requerimiento
                        peticion["regarding"] = new EntityReference("incident" , new Guid(requerimiento["incidentid"].ToString()));
                        //Petición Asignada a Ejecutivo resolutor
                        EntityReference ejecutivoresolutor = (EntityReference)requerimiento["new_ejecutivoresolutor"];
                        peticion["owner"] = new EntityReference("systemuser", ejecutivoresolutor.Id);
                        //Descripción heredada desde Requerimiento a Petición
                        peticion["new_descripcion"] = requerimiento["description"];
                        //Fecha vencimiento Petición - 10 días despues de la creación del registro
                        peticion["new_fecharesolucion"] = (DateTime)DateTime.Now.AddDays(10);
                        
                        //Otros Campos (No deberían quedar vacios)
                        peticion["new_name"] = "Petición creada desde Requerimiento: " + requerimiento["ticketnumber"];
                        peticion["subject"] = requerimiento["title"];


                        service.Create(peticion);
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidPluginExecutionException(ex.Message);
                }
            }
        }
    }
}
