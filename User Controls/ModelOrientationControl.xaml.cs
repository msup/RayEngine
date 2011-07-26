using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Plugin;
using WpfOpenTK.OpenGL.Engine;
using System.Windows.Input;
using System.Collections;
using System.Collections.Generic;
using WpfOpenTK.Managers;

namespace WpfOpenTK.User_Controls
{
    /// <summary>
    /// Interaction logic for ModelOrientationControl.xaml
    /// </summary>
    /// 

    public class Commands
    {
        public static readonly RoutedUICommand SaveOrientation =
            new RoutedUICommand("SaveOrientation", "SaveOrientation", typeof(ModelOrientationControl));
    }

    public partial class ModelOrientationControl : UserControl
    {
        public static RoutedCommand MyCommand = new RoutedCommand();

        // FIXME: change to xaml initialization
        private IRenderEngine renderPlugin;
        private RayCaster engine = null;
        private delegate void renderDelegate();
        private WindowManager wm = null;

        public ModelOrientationControl(ref RayCaster raycaster, WindowManager wm)
            : this(((IRenderEngine)raycaster),wm)
        {
           
        }

        public ModelOrientationControl(IRenderEngine renderPlugin, WindowManager wm)
        {
            InitializeComponent();

            this.wm = wm;

            this.renderPlugin = renderPlugin;
            engine = renderPlugin as RayCaster;

            CommandBinding cb = new CommandBinding(MyCommand, SaveOrientationExecuted, SaveOrientationCanExecute);
            this.CommandBindings.Add(cb);
            btnSavePosition.Command = MyCommand;

            KeyGesture kg = new KeyGesture(Key.M, ModifierKeys.Control);
            InputBinding ib = new InputBinding(MyCommand, kg);
            this.InputBindings.Add(ib);
        }

        private void btnRender_Click(object sender, RoutedEventArgs e)
        {
            var animation_points = this.engine.animationManager.GetAnimationList;
            int key_points_count = this.engine.animationManager.GetAnimationList.Count();

            foreach (var item in animation_points)
            {
                KeyValuePair<AnimationStep, ArrayList> temp = (KeyValuePair<AnimationStep, ArrayList>)item;

                this.engine.AngleX = (double)temp.Value[0];
                this.engine.AngleY = (double)temp.Value[1];
                this.engine.AngleZ = (double)temp.Value[2];

                #region remove possibly
                // ok working
                //if ( engine == null )
                //    throw new Exception( "Render Engine object is null." );
                //else
                //    Dispatcher.BeginInvoke( new renderDelegate( engine.Render ) );
                #endregion
                                
                engine.Render();

                var rwm = wm.GetRenderWindowManager;
                var glWindow = rwm.RenderWindows[0].GlWindows[0];
                
                glWindow.Render(0.00001f);
                RayCaster raycaster = glWindow.GetRenderEngine as RayCaster;
                if (raycaster != null)
                    raycaster.GrabScreenshot();

                //lock (this)
                //{
                //    Thread newThread = new Thread(ts);
                //    newThread.Priority = ThreadPriority.BelowNormal;
                //    newThread.Start();
                //}

            }
        }

        private void btnScreenshot_Click(object sender, RoutedEventArgs e)
        {
            var engine = renderPlugin as RayCaster;
            engine.GrabScreenshot();
        }

        // Commands
        private void SaveOrientationCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }

        private void SaveOrientationExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.engine.AngleX = this.sldXaxis.Value;
            this.engine.AngleY = this.sldYaxis.Value;
            this.engine.AngleZ = this.sldZaxis.Value;

            this.engine.animationManager.GetAnimationList.Push(Managers.AnimationStep.Rotations, sldXaxis.Value, sldYaxis.Value, sldZaxis.Value);

            this.labAnimationCount.Content = this.engine.animationManager.GetAnimationList.Count().ToString();
        }
    }
}