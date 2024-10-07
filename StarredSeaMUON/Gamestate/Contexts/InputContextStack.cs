using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarredSeaMUON.Gamestate.Contexts
{
    internal class InputContextStack
    {
        public const int maxCount = 64;
        public InputContext? baseContext;
        private Stack<InputContext> contexts = new Stack<InputContext>(maxCount);

        public void SwitchBase(InputContext newBaseContext)
        {
            if(baseContext != null)
                baseContext.OnDeactivate();
            baseContext = newBaseContext;
            baseContext.OnActivate();
        }

        public bool Push(InputContext context)
        {
            if (contexts.Count >= maxCount) return false;
            Peek().OnDeactivate();
            contexts.Push(context);
            Peek().OnActivate();
            return true;
        }

        public InputContext Pop()
        {
            if (contexts.Count > 0)
            {
                return contexts.Pop();
            }
            return baseContext;
        }

        public InputContext Peek()
        {
            if (contexts.Count > 0)
            {
                return contexts.Peek();
            }
            return baseContext;
        }

        public InputContext GetCurrent()
        {
            return Peek();
        }

        internal void Tick(GlobalTickEventArgs e)
        {

            GetCurrent()?.Tick(e.Delta);
        }
    }
}
