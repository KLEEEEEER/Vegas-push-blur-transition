using ScriptPortal.Vegas;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace Push_Blur_Transition
{
    enum TransitionMode
    {
        ToLeft,
        ToRight,
        ToTop,
        Down
    }

    enum TransitionVideo
    {
        First,
        Last
    }

    public class PushBlurTransitionModule : ICustomCommandModule
    {
        protected Vegas myVegas = null;

        private int windowWidth = 164;
        private int windowHeight = 600;
        private int buttonHeight = 30;
        private int buttonWidth = 142;
        private int paddingTop = 4;
        private int paddingBottom = 4;
        private int paddingLeft = 4;

        public void InitializeModule(Vegas vegas)
        {
            myVegas = vegas;
        }

        CustomCommand myViewCommand = new CustomCommand(CommandCategory.View, "mySampleViewCommand");

        public ICollection GetCustomCommands()
        {
            myViewCommand.DisplayName = "Blured Push";
            myViewCommand.Invoked += this.HandleInvoked;
            myViewCommand.MenuPopup += this.HandleMenuPopup;
            return new CustomCommand[] { myViewCommand };
        }

        void HandleInvoked(Object sender, EventArgs args)
        {
            if (!myVegas.ActivateDockView("PushBlurExt"))
            {
                DockableControl dockView = new DockableControl("PushBlurExt");
                System.Windows.Forms.Label label = new System.Windows.Forms.Label();

                dockView.DefaultFloatingSize = new System.Drawing.Size(windowWidth, windowHeight);

                int filledSpace = buttonHeight + paddingTop + paddingBottom;

                Button pushLeft = new Button();
                pushLeft.Text = "From Right";
                pushLeft.Click += pushRightClick;
                pushLeft.Location = new System.Drawing.Point(paddingLeft, filledSpace * 1 + paddingTop);
                pushLeft.Width = buttonWidth;
                pushLeft.Height = buttonHeight;
                //pushLeft.Dock = DockStyle.Fill;
                dockView.Controls.Add(pushLeft);

                Button pushRight = new Button();
                pushRight.Text = "From Left";
                pushRight.Click += pushLeftClick;
                pushRight.Location = new System.Drawing.Point(paddingLeft, filledSpace * 2 + paddingTop);
                pushRight.Width = buttonWidth;
                pushRight.Height = buttonHeight;
                dockView.Controls.Add(pushRight);

                //label.Dock = DockStyle.Fill;
                label.Text = "PushBlurExt 0.1";
                label.Location = new System.Drawing.Point(paddingLeft, paddingTop);
                label.Width = buttonWidth;
                label.Height = buttonHeight;
                label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                dockView.Controls.Add(label);


                dockView.Width = 600;

                myVegas.LoadDockView(dockView);
            }
        }

        void HandleMenuPopup(Object sender, EventArgs args)
        {
            myViewCommand.Checked = myVegas.FindDockView("PushBlurExt");
        }

        private void pushRightClick(object sender, System.EventArgs e)
        {
            List<TrackEvent> selectedMedias = VegasHelper.FindSelectedEvents(myVegas.Project);
            selectedMedias = selectedMedias.OrderBy((x) => x.Start).ToList();

            if (!validateSelectedTrackEvents(selectedMedias)) return;

            PushBlur pushBlur = new PushBlur(selectedMedias, myVegas, TransitionMode.ToLeft);
            pushBlur.applyEffects();
        }

        private void pushLeftClick(object sender, System.EventArgs e)
        {
            List<TrackEvent> selectedMedias = VegasHelper.FindSelectedEvents(myVegas.Project);
            selectedMedias = selectedMedias.OrderBy((x) => x.Start).ToList();

            if (!validateSelectedTrackEvents(selectedMedias)) return;

            PushBlur pushBlur = new PushBlur(selectedMedias, myVegas, TransitionMode.ToRight);
            pushBlur.applyEffects();
        }

        private bool validateSelectedTrackEvents(List<TrackEvent> selectedMedias)
        {
            if (selectedMedias.Count != 2)
            {
                MessageBox.Show($"Need to select 2 elements. Selected " + selectedMedias.Count + " elements.");
                return false;
            }

            if (VegasHelper.isPickedEventsInTheSameGroup(selectedMedias))
            {
                MessageBox.Show($"Need to select 2 elements from separate groups.");
                return false;
            }

            return true;
        }
    }
}
