#region FileInfo

// 
// File: DragAdorner.cs
// 
// 
// ============================================================
// ============================================================
// 
// 
// Copyright (c) 2016, Teriks
// 
// All rights reserved.
// 
// 
// This file is part of LibLSLCC.
// 
// LibLSLCC is distributed under the following BSD 3-Clause License
// 
// 
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
// 
// 1. Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
// 
// 2. Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer
//     in the documentation and/or other materials provided with the distribution.
// 
// 3. Neither the name of the copyright holder nor the names of its contributors may be used to endorse or promote products derived
//     from this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
// HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
// LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
// ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// 
// 
// ============================================================
// ============================================================
// 
// 

#endregion

#region Imports

using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

#endregion

namespace LSLCCEditor.Utility.Wpf
{
    public class DragAdorner : Adorner
    {
        private readonly AdornerLayer _adornerLayer;
        private readonly UIElement _adornment;
        private Point _mousePos;


        public Point MousePosition
        {
            get { return _mousePos; }
            set
            {
                if (_mousePos == value) return;

                _mousePos = value;
                _adornerLayer.Update(AdornedElement);
            }
        }



        public DragAdorner(UIElement adornedElement, UIElement adornment)
            : base(adornedElement)
        {
            _adornerLayer = AdornerLayer.GetAdornerLayer(adornedElement);
            _adornerLayer.Add(this);
            _adornment = adornment;
            IsHitTestVisible = false;
        }




        protected override int VisualChildrenCount
        {
            get { return 1; }
        }


        public void Remove()
        {
            _adornerLayer.Remove(this);
        }


        protected override Size ArrangeOverride(Size finalSize)
        {
            _adornment.Arrange(new Rect(finalSize));
            return finalSize;
        }


        public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
        {
            var result = new GeneralTransformGroup();
            result.Children.Add(base.GetDesiredTransform(transform));
            result.Children.Add(new TranslateTransform(MousePosition.X - 4, MousePosition.Y - 4));
            return result;
        }


        protected override Visual GetVisualChild(int index)
        {
            return _adornment;
        }


        protected override Size MeasureOverride(Size constraint)
        {
            _adornment.Measure(constraint);
            return _adornment.DesiredSize;
        }
    }
}