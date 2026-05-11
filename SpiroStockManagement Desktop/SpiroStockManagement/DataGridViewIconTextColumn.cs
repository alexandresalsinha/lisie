using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;

namespace SpiroStockManagement
{
    public class DataGridViewIconTextColumn : DataGridViewTextBoxColumn
    {
        private Image imageValue;
        private Size imageSize;

        public DataGridViewIconTextColumn()
        {
            this.CellTemplate = new DataGridViewIconTextCell();
        }

        public override object Clone()
        {
            DataGridViewIconTextColumn c = base.Clone() as DataGridViewIconTextColumn;
            c.imageValue = this.imageValue;
            c.imageSize = this.imageSize;

            return c;
        }

        public Image Image
        {
            get { return this.imageValue; }
            set
            {
                if (this.Image != value)
                {
                    this.imageValue = value;
                    this.imageSize = value.Size;

                    if (this.InheritedStyle != null)
                    {
                        Padding inheritedPadding = this.InheritedStyle.Padding;
                        this.DefaultCellStyle.Padding = new Padding(inheritedPadding.Left,
                            inheritedPadding.Top, imageSize.Width,
                            inheritedPadding.Bottom);
                    }
                }
            }
        }

        private DataGridViewIconTextCell TextAndImageCellTemplate
        {
            get { return this.CellTemplate as DataGridViewIconTextCell; }
        }

        internal Size ImageSize
        {
            get { return imageSize; }
        }
    }
}