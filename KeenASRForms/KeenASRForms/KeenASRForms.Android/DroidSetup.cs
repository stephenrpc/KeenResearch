using Autofac;
using KeenASRForms.Droid.Services;
using KeenASRForms.Interfaces;

namespace KeenASRForms.Droid
{
    public class DroidSetup : AppSetup
    {
        protected override void RegisterDepenencies(ContainerBuilder cb)
        {
            base.RegisterDepenencies(cb);

            cb.RegisterType<ASR_Android>().As<IASR>().SingleInstance();
        }
    }
}