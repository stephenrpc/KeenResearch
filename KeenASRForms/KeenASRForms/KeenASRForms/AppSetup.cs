using Autofac;

namespace KeenASRForms
{
    // we can inherit and override this class on each platform to make services specific
    public class AppSetup
    {
        public string PackageFolder { get; set; }
        public string ImportPackagePath { get; set; }

        /// <summary>
        /// Creates an instance of the AutoFac container
        /// </summary>
        /// <returns>A new instance of the AutoFac container</returns>
        /// <remarks>
        /// https://github.com/autofac/Autofac/wiki
        /// </remarks>
        public IContainer CreateContainer()
        {
            var cb = new ContainerBuilder();

            RegisterDepenencies(cb);
            return cb.Build();
        }

        protected virtual void RegisterDepenencies(ContainerBuilder cb)
        {

        }

    }
}
