using System;
using System.Collections.Generic;
using System.Text;

namespace FluxionEditor.Foundation.Utilities
{
    public static class ID
    {
        public static int INVALID_ID = -1;
        public static bool isValid(int id) => id != INVALID_ID;
    }
}
