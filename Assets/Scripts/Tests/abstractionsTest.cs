using System;
using UnityEngine;

namespace Scripts.Tests
{
    public class Parent
    {
        protected internal virtual void SetSomething()
        {
            
        }
    }
    
    public class Child: Parent
    {

        sealed protected internal override void SetSomething()
        {
            
        }
    }
    
    public class GrandChild: Child
    {
        private void Start()
        {
            // in this case should be an error
            SetSomething();
            
        }
    }
}