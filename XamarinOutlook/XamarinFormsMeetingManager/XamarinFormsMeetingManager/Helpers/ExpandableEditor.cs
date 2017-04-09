//Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.
//See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace XamarinFormsMeetingManager.Helpers
{
    //Inherits from Editor class, but this version resizes as user types. 
    //See http://forums.xamarin.com/discussion/21951/allow-the-editor-control-to-grow-as-content-lines-are-added#latest
    public class ExpandableEditor : Editor
    {
        public ExpandableEditor() : base()
 {
        }

        public void InvalidateLayout()
        {
            this.InvalidateMeasure();
        }


    }
}
