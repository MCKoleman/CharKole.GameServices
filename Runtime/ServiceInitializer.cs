using System;
using System.Collections.Generic;
using UnityEngine;

namespace CharKole.GameServices
{
    public class ServiceInitializer : MonoBehaviour
    {
        protected virtual bool LOG_DEBUG => false;
        protected virtual bool LOG_WARN => true;
        protected virtual bool LOG_ERROR => true;
        
        private Dictionary<Type, GameService> services = new();

        public static ServiceInitializer Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(this.gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(this.gameObject);

            RegisterServices();
            InitServices();
        }

        // Override this method in your implementation to register services in the desired order
        protected virtual void RegisterServices() { }

        // Use this method to register the GameServices that are children of this object
        protected void RegisterService<T>() where T : GameService
        {
            T service = this.GetComponentInChildren<T>();
            if (service == null)
            {
                if (LOG_ERROR)
                {
                    Debug.LogError($"[CharKole.GameService.ServiceInitializer]: Service {typeof(T)} does not exist on ServiceInitializer.");
                }
                return;
            }

            if (services.ContainsKey(service.GetType())) 
            {
                if (LOG_WARN)
                {
                    Debug.LogWarning($"[CharKole.GameService.ServiceInitializer]: Skipped registering service {service.name}, a service of the same type already exists.");
                }
                return;
            }

            if (LOG_DEBUG)
            {
                Debug.Log($"[CharKole.GameService.ServiceInitializer]: Registering service {service.name}.");
            }
            ServiceLocator.Register<T>(service);
            services.Add(service.GetType(), service);
        }

        private void InitServices()
        {
            foreach (GameService service in services.Values)
            {
                if (LOG_DEBUG)
                {
                    Debug.Log($"[CharKole.GameService.ServiceInitializer]: Initializing service {service.name}.");
                }
                service.Init();
            }
        }
    }
}
