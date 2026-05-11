using System;
using System.Collections.Generic;
using System.Text;
// Add these to your file:
using System.Drawing;
using System.Windows.Forms;

namespace SpiroStockManagement
{
    public class DataGridViewIconTextCell : DataGridViewTextBoxCell
    {
        private Image imageValue;
        private Size imageSize;

        public override object Clone()
        {
            DataGridViewIconTextCell c = base.Clone() as DataGridViewIconTextCell;
            c.imageValue = this.imageValue;
            c.imageSize = this.imageSize;
            return c;
        }

        public Image Image
        {
            get
            {
                if (this.OwningColumn == null ||
                 this.OwningTextAndImageColumn == null)
                {
                    return imageValue;
                }
                else if (this.imageValue != null)
                {
                    return this.imageValue;
                }
                else
                {
                    return this.OwningTextAndImageColumn.Image;
                }
            }
            set
            {
                if (this.imageValue != value)
                {
                    try
                    {
                        this.imageValue = value;
                        //this.imageValue = value;
                        this.imageSize = this.imageValue.Size;
                        //this.imageSize = new S

                        Padding inheritedPadding = this.InheritedStyle.Padding;
                        this.Style.Padding = new Padding(inheritedPadding.Left,
                        inheritedPadding.Top, imageSize.Width,
                        inheritedPadding.Bottom);
                    }
                    catch (Exception exp)
                    {
                        MessageBox.Show(exp.Message);
                    }
                }
            }
        }

        protected override void Paint(Graphics graphics, Rectangle clipBounds,
            Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState,
            object value, object formattedValue, string errorText,
            DataGridViewCellStyle cellStyle,
            DataGridViewAdvancedBorderStyle advancedBorderStyle,
            DataGridViewPaintParts paintParts)
        {
            // Paint the base content
            base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState,
               value, formattedValue, errorText, cellStyle,
               advancedBorderStyle, paintParts);

            if (this.Image != null)
            {
                // Draw the image clipped to the cell.
                System.Drawing.Drawing2D.GraphicsContainer container =
                graphics.BeginContainer();

                graphics.SetClip(cellBounds);
                Rectangle rect = new Rectangle();
                rect.Location = new Point(cellBounds.Location.X + cellBounds.Width - this.Image.Width - 1, cellBounds.Location.Y + 1);
                rect.Size = new Size(this.Image.Width, cellBounds.Height - 2);

                //Draw image scaled, the image will be resized to fit the cell
                graphics.DrawImage(this.Image, rect);

                //Draw image unscaled, the image size will keep unchanged.
                //graphics.DrawImageUnscaled(this.Image, rect);

                graphics.EndContainer(container);
            }
        }

        private DataGridViewIconTextColumn OwningTextAndImageColumn
        {
            get { return this.OwningColumn as DataGridViewIconTextColumn; }
        }
    }
}
