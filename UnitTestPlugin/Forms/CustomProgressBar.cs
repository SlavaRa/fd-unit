using System.Drawing;
using System.Windows.Forms;

namespace TestExplorerPanel.Forms {

    class CustomProgressBar : ProgressBar {

        private SolidBrush foregroundBrush;

        public CustomProgressBar() {
            this.SetStyle( ControlStyles.UserPaint , true );
        }

        protected override void OnPaintBackground( PaintEventArgs e ) {

        }

        protected override void OnPaint( PaintEventArgs e ) {
            foregroundBrush = new SolidBrush( this.ForeColor );

            Rectangle drawArea = GetDrawArea();

            if ( ProgressBarRenderer.IsSupported )
                ProgressBarRenderer.DrawHorizontalBar( e.Graphics , drawArea );
                
            drawArea.Width = ( int ) ( drawArea.Width * ( ( double ) Value / Maximum ) ) - 4;
            drawArea.Height = drawArea.Height - 4;

            e.Graphics.FillRectangle( foregroundBrush , 2 , 2 , drawArea.Width , drawArea.Height );
        }

        private Rectangle GetDrawArea() {
            return new Rectangle( 0 , 0 , this.Width , this.Height );
        }
    }
}
