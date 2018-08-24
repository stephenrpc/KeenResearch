using Autofac;
using KeenASRForms.Interfaces;
using KeenASRForms.iOS.Services;

namespace KeenASRForms.iOS
{
    public class IOSSetup : AppSetup
    {
        protected override void RegisterDepenencies(ContainerBuilder cb)
        {
            base.RegisterDepenencies(cb);
            cb.RegisterType<ASR_iOS>().As<IASR>().SingleInstance();
        }
    }
}