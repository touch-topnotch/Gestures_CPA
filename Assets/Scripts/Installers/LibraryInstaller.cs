using System;
using Scripts.Gestures;
using UnityEngine;
using Zenject;

namespace Scripts.Installers
{
    public class LibraryInstaller: MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<GesturesLibrary>().FromNew().AsSingle().NonLazy();
        }
    }
}