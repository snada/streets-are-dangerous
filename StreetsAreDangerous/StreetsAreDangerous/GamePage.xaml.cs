using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Advertising.Mobile.Xna;
using Microsoft.Advertising;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Phone.Shell;
using Microsoft.Devices;
using System.IO;
using System.IO.IsolatedStorage;
using System.Xml.Serialization;
using StreetsAreDangerous.Resources;
using StreetsAreDangerous.Enemies;
using StreetsAreDangerous.Elements;
using StreetsAreDangerous.PowerUps;
using StreetsAreDangerous.Utilities;
using StreetsAreDangerous.Scores;
using StreetsAreDangerous.Saves;

namespace StreetsAreDangerous
{
    public partial class GamePage : PhoneApplicationPage
    {
        //Pubblicità
        DrawableAd drawableAd;

        //Logica di gioco
        SavedMatch match;

        ContentManager contentManager;
        GameTimer timer;
        SpriteBatch spriteBatch;
        UIElementRenderer silverlightRenderer;

        //Vita
        private int maxLife;

        //Punteggi
        private Score[] scores;

        /*
        private bool debug;
        */

        //Speed
        private Curve speedCurve;
        private CurveKey firstKey;

        //InputManager
        InputManager inputManager;

        //Font
        private SpriteFont font;

        //Modelli 3D
        private Model[] models;

        //Textures 2D
        private Texture2D timeBack;
        private Texture2D barraBack;
        private Texture2D barra;
        private Texture2D heart;

        //Suoni
        private float soundEffectsVolume;
        private SoundEffect bark;
        private SoundEffect hit;

        //Vibrazione
        private bool vibration;

        //Brightness-Contrast per flashes del boost
        Curve boostFlashCurve;
        private Texture2D whiteTexture;
        private int brightness;
        private BlendState brightnessBlend;
        private BlendState contrastBlend;

        //Nero per fade finale
        private Texture2D blackTexture;

        //Nome per il salvataggio
        private string name;

        public GamePage()
        {
            InitializeComponent();

            //Testo dei componenti nella giusta lingua
            GameOverBackButton.Content = Strings.GameOverButton;
            GameOverTextBlock.Text = Strings.GameOver;
            GameOverNoRecordTextBlock.Text = Strings.NoRecord;

            contentManager = (Application.Current as App).Content;

            this.LayoutUpdated += new EventHandler(GamePage_LayoutUpdated);
            this.GoBackGameOverStoryBoard.Completed += new EventHandler(GoBackGameOverStoryBoard_Completed);

            timer = new GameTimer();
            timer.UpdateInterval = TimeSpan.FromTicks(333333);
            timer.Update += OnUpdate;
            timer.Draw += OnDraw;

            SharedGraphicsDeviceManager.Current.GraphicsDevice.PresentationParameters.IsFullScreen = true;
        }

        void GoBackGameOverStoryBoard_Completed(object sender, EventArgs e)
        {
            match.gameState = GamePageState.HAVETOEXIT;
            this.OnBackKeyPress(new System.ComponentModel.CancelEventArgs(false));
        }

        void GamePage_LayoutUpdated(object sender, EventArgs e)
        {
            if (null == silverlightRenderer)
                silverlightRenderer = new UIElementRenderer(this, 800, 480);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SharedGraphicsDeviceManager.Current.GraphicsDevice.SetSharingMode(true);

            spriteBatch = new SpriteBatch(SharedGraphicsDeviceManager.Current.GraphicsDevice);

            /**
            DebugShapeRenderer.Initialize(SharedGraphicsDeviceManager.Current.GraphicsDevice);
            */

            //LA VITA DEVE CAMBIARE A SECONDA DEL LIVELLO DI DIFFICOLTA'
            maxLife = Constants.MAX_EASY_LIFE;

            //SE NON ESISTE UN MATCH CARICATO LO CREO. MA SOLO ALLORA.
            if (((App)App.Current).NewMatch)
            {
                ((App)App.Current).match = new SavedMatch();
                ((App)App.Current).match.cameraPos = new Vector3(0, 3.5f, 0);
                ((App)App.Current).match.cameraLook = new Vector3(0, 3.5f, -1);
                ((App)App.Current).match.cameraShake = Vector2.Zero;
                ((App)App.Current).match.shakeMagnitude = 0;
                ((App)App.Current).match.playerBox = new BoundingBox(new Vector3(((App)App.Current).match.cameraPos.X, ((App)App.Current).match.cameraPos.Y, ((App)App.Current).match.cameraPos.Z - 1), ((App)App.Current).match.cameraPos);
                ((App)App.Current).match.life = Utilities.Constants.MAX_EASY_LIFE;

                ((App)App.Current).match.lifepositions = new float[maxLife];
                for (int counter = 0; counter < ((App)App.Current).match.lifepositions.Length; counter++)
                    ((App)App.Current).match.lifepositions[counter] = 0;

                ((App)App.Current).match.matchTime = new TimeSpan();
                ((App)App.Current).match.gameState = GamePageState.PLAY;

                //Add sections
                ((App)App.Current).match.sections = new List<Section>();
                ((App)App.Current).match.sections.Add(new Section(0, 0, 0));
                ((App)App.Current).match.sections.Add(new Section(0, 0, ((App)App.Current).match.sections[((App)App.Current).match.sections.Count - 1].zPosition - 50));
                ((App)App.Current).match.sections.Add(new Section(0, 0, ((App)App.Current).match.sections[((App)App.Current).match.sections.Count - 1].zPosition - 50));

                ((App)App.Current).match.speedTime = 0.1f;
                ((App)App.Current).match.flashCounter = 0;
                ((App)App.Current).match.fadeIndex = 0;
            }
            else
                ((App)App.Current).NewMatch = true;

            match = ((App)App.Current).match;

            //Pubblicità
            AdComponent.Initialize("00000000-0000-0000-0000-000000000000");
            drawableAd = AdComponent.Current.CreateAd("00000000", new Rectangle(168, 0, 480, 80), true);
            drawableAd.AdRefreshed += new EventHandler(drawableAd_AdRefreshed);
            drawableAd.ErrorOccurred += new EventHandler<AdErrorEventArgs>(drawableAd_ErrorOccurred);

            //Textures
            timeBack = contentManager.Load<Texture2D>("TimeBack");
            barraBack = contentManager.Load<Texture2D>("BarraBack");
            barra = contentManager.Load<Texture2D>("Barra");
            heart = contentManager.Load<Texture2D>("Heart");

            //Models
            models = new Model[8];
            models[0] = contentManager.Load<Model>("planeModel");
            models[1] = contentManager.Load<Model>("fenceModel");
            models[2] = contentManager.Load<Model>("treeModel");
            models[3] = contentManager.Load<Model>("dogModel");
            models[4] = contentManager.Load<Model>("shadowModel");
            models[5] = contentManager.Load<Model>("boostModel");
            models[6] = contentManager.Load<Model>("heartModel");
            models[7] = contentManager.Load<Model>("boxesModel");

            //Suoni
            bark = contentManager.Load<SoundEffect>("bark");
            hit = contentManager.Load<SoundEffect>("hit");

            //Font
            font = contentManager.Load<SpriteFont>("font");

            /*
            debug = false;
            */

            //Speed - abbastanza definitivo.
            speedCurve = new Curve();
            firstKey = new CurveKey(0, 0);
            speedCurve.Keys.Add(firstKey);

            speedCurve.Keys.Add(new CurveKey(Constants.SPRINT_DURATION, Constants.MAX_SPEED));
            speedCurve.Keys.Add(new CurveKey(Constants.SPRINT_DURATION + Constants.MAX_SPEED_DURATION, Constants.MAX_SPEED));
            speedCurve.Keys.Add(new CurveKey(Constants.SPRINT_DURATION + Constants.MAX_SPEED_DURATION + Constants.SLOWDOWN_DURATION, 0));

            //Flash boost
            boostFlashCurve = new Curve();
            boostFlashCurve.Keys.Add(new CurveKey(0, 128));
            boostFlashCurve.Keys.Add(new CurveKey(Constants.FLASH_DURATION - Constants.FLASH_DURATION / 5, Constants.FLASH_STRENGTH));
            boostFlashCurve.Keys.Add(new CurveKey(Constants.FLASH_DURATION, 128));

            inputManager = new InputManager(true);

            //FLASH PICKUP BOOST
            whiteTexture = new Texture2D(SharedGraphicsDeviceManager.Current.GraphicsDevice, 1, 1);
            whiteTexture.SetData<Color>(new Color[] { Color.White });
            brightness = 255;
            brightnessBlend = new BlendState();
            brightnessBlend.ColorSourceBlend = brightnessBlend.AlphaSourceBlend = Blend.Zero;
            brightnessBlend.ColorDestinationBlend = brightnessBlend.AlphaDestinationBlend = Blend.SourceColor;
            contrastBlend = new BlendState();
            contrastBlend.ColorSourceBlend = contrastBlend.AlphaSourceBlend = Blend.DestinationColor;
            contrastBlend.ColorDestinationBlend = contrastBlend.AlphaDestinationBlend = Blend.SourceColor;

            //BLACK FADE
            blackTexture = new Texture2D(SharedGraphicsDeviceManager.Current.GraphicsDevice, 1, 1);
            blackTexture.SetData<Color>(new Color[] { Color.Black });

            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            soundEffectsVolume = (float)settings["SoundsVolume"];
            vibration = (bool)settings["Vibration"];

            timer.Start();
            PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            base.OnNavigatedTo(e);
        }

        void drawableAd_ErrorOccurred(object sender, AdErrorEventArgs e)
        {

        }

        void drawableAd_AdRefreshed(object sender, EventArgs e)
        {

        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (!e.Cancel && match.gameState != GamePageState.HAVETOEXIT)
            {
                if (match.gameState == GamePageState.LOST)
                {
                    e.Cancel = true;
                    GoBackGameOverStoryBoard.Begin();
                }
                else
                {
                    match.previousState = match.gameState;
                    match.gameState = GamePageState.PAUSE;
                    PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Enabled;
                    if (MessageBox.Show(Strings.MsgBoxExitText, Strings.MsgBoxExitCaption, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                    {
                        e.Cancel = true;
                        match.gameState = match.previousState;
                        PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
                    }
                    else
                    {
                        inputManager = null;
                        ((App)App.Current).match = null;
                        NavigationService.GoBack();
                    }
                }
            }
            else
            {
                IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication();
                //
                //
                //CODICE PER IL SALVATAGGIO O MENO DEI TEMPI ALL'USCITA
                //
                //
                //Il tempo è un record?
                if (scores.Length == 0 || scores.Length < Constants.NUMBER_OF_RECORDS_SAVED || new Score(match.matchTime, "").CompareTo(scores[scores.Length - 1]) < 0)
                {
                    //IMPORTANTE: LA RISCRITTURA DEL FILE PUO' LASCIARE ERRORI. ELIMINARLO E RISCRIVERLO
                    storage.DeleteFile("scores.xml");

                    //SCRIVO LA NUOVA LISTA DI MATCH.
                    using (IsolatedStorageFileStream stream = storage.OpenFile("scores.xml", FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        XmlSerializer xml = new XmlSerializer(typeof(Score[]));

                        {
                            Score[] tmp = new Score[scores.Length + 1];
                            Array.Copy(scores, tmp, scores.Length);
                            string tmpName;

                            if (GameOverNameTextBox.Text.Trim() == "")
                            {
                                GameOverNameTextBox.Text = name;
                                tmpName = name;
                            }
                            else
                            {
                                tmpName = GameOverNameTextBox.Text.Trim();
                                IsolatedStorageSettings.ApplicationSettings["DefaultName"] = GameOverNameTextBox.Text.Trim();
                            }

                            tmp[scores.Length] = new Score(match.matchTime, tmpName);
                            scores = tmp;
                            Array.Sort(scores);
                        }

                        //TRONCO LE PARTITE OLTRE IL LIMITE DI SALVATAGGI CONSENTITI.
                        if (scores.Length > Constants.NUMBER_OF_RECORDS_SAVED)
                        {
                            Score[] tmp = new Score[scores.Length - 1];
                            Array.Copy(scores, tmp, tmp.Length);
                            scores = tmp;
                        }

                        xml.Serialize(stream, scores);
                        stream.Close();
                        stream.Dispose();
                    }
                    storage.Dispose();
                }
                ((App)App.Current).match = null;
                NavigationService.GoBack();
            }
            base.OnBackKeyPress(e);
        }

        /// <summary>
        /// Metodo che viene eseguito quando si naviga VIA da questa pagina.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // Arrestare il timer
            timer.Stop();

            GC.SuppressFinalize(this);

            /*DebugShapeRenderer.Kill();*/

            // Impostare la modalità di condivisione del dispositivo grafico per disattivare il rendering XNA
            SharedGraphicsDeviceManager.Current.GraphicsDevice.SetSharingMode(false);
            base.OnNavigatedFrom(e);
        }

        /// <summary>
        /// Consente alla pagina di eseguire logica come l'aggiornamento del mondo,
        /// il controllo dei conflitti, la raccolta delle informazioni di input e la riproduzione dell'audio.
        /// </summary>
        private void OnUpdate(object sender, GameTimerEventArgs e)
        {
            AdComponent.Current.Update(e.ElapsedTime);

            if (inputManager == null)
                inputManager = new InputManager(true);

            if (match.gameState == GamePageState.PLAY)
            {
                match.matchTime += e.ElapsedTime;

                //Gestione sections
                if(match.sections[0].zPosition >= 50)
                {
                    match.sections.RemoveAt(0);
                    match.sections.Add(new Section(0, UtilityClass.numberOfEnemies(match.matchTime.TotalSeconds), match.sections[match.sections.Count - 1].zPosition - 50));
                }

                if (match.flashCounter > 0)
                    match.flashCounter -= e.ElapsedTime.Milliseconds / 1000.0f;

                float speed = speedCurve.Evaluate(match.speedTime);
                if (speed > 0 && match.life > 0)
                {
                    //Aggiornamento posizione.
                    if ((App.Current.RootVisual as PhoneApplicationFrame).Orientation == PageOrientation.LandscapeRight)
                    {
                        match.cameraPos = new Vector3(MathHelper.Clamp(match.cameraPos.X + inputManager.accelerometerReading.Y * 2, -8.5f, 8.5f), match.cameraPos.Y, match.cameraPos.Z);
                        match.cameraLook = new Vector3(MathHelper.Clamp(match.cameraLook.X + inputManager.accelerometerReading.Y * 2, -8.5f, 8.5f), match.cameraLook.Y, match.cameraLook.Z); ;
                    }
                    else
                    {
                        match.cameraPos = new Vector3(MathHelper.Clamp(match.cameraPos.X - inputManager.accelerometerReading.Y * 2, -8.5f, 8.5f), match.cameraPos.Y, match.cameraPos.Z);
                        match.cameraLook = new Vector3(MathHelper.Clamp(match.cameraLook.X - inputManager.accelerometerReading.Y * 2, -8.5f, 8.5f), match.cameraLook.Y, match.cameraLook.Z); ;
                    }

                    //Aggiornamento boundingBox del player.
                    match.playerBox = new BoundingBox(new Vector3(match.cameraPos.X - 0.5f, 0, match.cameraPos.Z - 1), new Vector3(match.cameraPos.X + 0.5f, match.cameraPos.Y, match.cameraPos.Z + 3));

                    //Shake per un impatto con nemico
                    if (match.shakeMagnitude == 0)
                        match.cameraShake = Vector2.Zero;
                    else
                    {
                        float x = UtilityClass.nextInt(0, match.shakeMagnitude);
                        float y = UtilityClass.nextInt(0, match.shakeMagnitude);
                        if (UtilityClass.nextBoolean())
                            x = -x;
                        if (UtilityClass.nextBoolean())
                            y = -y;
                        match.cameraShake = new Vector2(x / 10000, y / 10000);
                        match.shakeMagnitude -= 100;
                    }

                    foreach (Section s in match.sections)
                    {
                        s.Update(speedCurve.Evaluate(match.speedTime));
                        foreach(Vector3 emitterpos in s.emitters)
                        {
                            SoundEffectInstance instance = bark.CreateInstance();

                            AudioListener listener = new AudioListener();
                            listener.Position = match.cameraPos;
                            listener.Forward = match.cameraLook;
                            listener.Velocity = new Vector3(0, 0, speed);

                            AudioEmitter emitter = new AudioEmitter();
                            emitter.Position = new Vector3(emitterpos.X / 2.0f, emitterpos.Y, emitterpos.Z / 6.0f);
                            emitter.Forward = listener.Position;

                            instance.Pitch = (float)UtilityClass.nextInt(-5000, 5000) / 10000.0f;

                            instance.Volume = soundEffectsVolume;
                            instance.Apply3D(listener, emitter);
                            instance.Play();
                        }
                        s.emitters.Clear();
                        #region DEBUGCODE
                        /*
                        *
                        * DEBUG SHAPE
                        *
                        *
                        if (debug)
                        {
                            for (int counter = 0; counter < s.Enemies.Length; counter++)
                                if (s.Enemies[counter].IsAlive)
                                    DebugShapeRenderer.AddBoundingBox(s.Enemies[counter].boundingBox, Color.White);
                            for (int counter = 0; counter < s.PowerUps.Length; counter++)
                                if (s.PowerUps[counter].isAvailable)
                                    DebugShapeRenderer.AddBoundingBox(s.PowerUps[counter].boundingBox, Color.Green);
                        }
                        */
                        #endregion
                    }

                    //Collisioni con player e powerups
                    {
                        bool check = true;
                        Section section = match.sections[0];

                        while (check)
                        {
                            for (int counter = 0; counter < section.PowerUps.Length && section.PowerUps[counter].boundingBox.Max.Z >= match.playerBox.Min.Z; counter++)
                                if (section.PowerUps[counter].isAvailable && match.playerBox.Intersects(section.PowerUps[counter].boundingBox))
                                {
                                    section.PowerUps[counter].isAvailable = false;
                                    switch (section.PowerUps[counter].Model)
                                    {
                                        //Collisione con player e boost
                                        case 5:
                                            match.flashCounter = Constants.FLASH_DURATION;
                                            firstKey.Value = speedCurve.Evaluate(match.speedTime);
                                            match.speedTime = 0.1f;
                                            break;
                                        case 6:
                                            if (match.life < maxLife)
                                            {
                                                match.flashCounter = Constants.FLASH_DURATION;
                                                firstKey.Value = speedCurve.Evaluate(match.speedTime);
                                                match.life++;
                                            }
                                            break;
                                    }
                                }
                            check = false;

                            if (section.zPosition >= 47)
                            {
                                section = match.sections[1];
                                check = true;
                            }
                        }
                    }


                    //Collisioni con player e nemici
                    {
                        bool check = true;
                        Section section = match.sections[0];

                        while (check)
                        {
                            for (int counter = 0; counter < section.Enemies.Length && section.Enemies[counter].boundingBox.Max.Z >= match.playerBox.Min.Z; counter++)
                                if (section.Enemies[counter].IsAlive && match.playerBox.Intersects(section.Enemies[counter].boundingBox))
                                {
                                    section.Enemies[counter].IsAlive = false;
                                    match.life--;
                                    match.shakeMagnitude = 1000;

                                    if(vibration)
                                        VibrateController.Default.Start(new TimeSpan(0, 0, 0, 0, 500));

                                    SoundEffectInstance instance = hit.CreateInstance();
                                    instance.Pitch = (float)UtilityClass.nextInt(-5000, 5000) / 10000.0f;
                                    instance.Volume = soundEffectsVolume;
                                    instance.Play();
                                }
                            check = false;

                            if (section.zPosition >= 47)
                            {
                                section = match.sections[1];
                                check = true;
                            }
                        }
                    }
                    match.speedTime += e.ElapsedTime.Seconds + (float)e.ElapsedTime.Milliseconds / 1000;
                    speed = speedCurve.Evaluate(match.speedTime);

                    //Aggiornamento GUI cuori
                    for (int counter = 0; counter < match.lifepositions.Length; counter++)
                    {
                        if (counter < match.life)
                            match.lifepositions[counter] = MathHelper.Clamp(match.lifepositions[counter] - Constants.LIFE_TRANSITION_STEP, 0, heart.Height);
                        if (counter >= match.life)
                            match.lifepositions[counter] = MathHelper.Clamp(match.lifepositions[counter] + Constants.LIFE_TRANSITION_STEP, 0, heart.Height);
                    }

                    inputManager.Update();
                }
                else
                {
                    match.gameState = GamePageState.LOSING; //SI HA APPENA PERSO IL MATCH!
                    IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication();

                    IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
                    name = (string)settings["DefaultName"];
                    GameOverNameTextBox.Text = name;

                    using (IsolatedStorageFileStream stream = storage.OpenFile("scores.xml", FileMode.Open))
                    {
                        XmlSerializer xml = new XmlSerializer(typeof(Score[]));
                        scores = xml.Deserialize(stream) as Score[];
                        stream.Close();
                        stream.Dispose();
                    }
                    //Scrivo il tempo nella casella.
                    GameOverTimeTextBlock.Text = ((int)match.matchTime.TotalHours).ToString() + ":" + match.matchTime.Minutes.ToString() + ":" + match.matchTime.Seconds.ToString();
                    //Il tempo è un record?
                    if (scores.Length == 0 || scores.Length < Constants.NUMBER_OF_RECORDS_SAVED || new Score(match.matchTime, "").CompareTo(scores[scores.Length - 1]) < 0)
                        GameOverNoRecordTextBlock.Visibility = System.Windows.Visibility.Collapsed; //GUI UPDATE
                    else
                    {
                        //GUI UPDATE
                        GameOverYourNameTextBlock.Visibility = System.Windows.Visibility.Collapsed;
                        GameOverNameTextBox.Visibility = System.Windows.Visibility.Collapsed;
                    }
                }
            }
            else if (match.gameState == GamePageState.LOSING)
            {
                match.fadeIndex = MathHelper.Clamp(match.fadeIndex + Constants.BLACK_FADE_STEP, 0, 1);
                if (match.fadeIndex == 1)
                {
                    GameOverStoryBoard.Begin();
                    match.gameState = GamePageState.LOST;
                }
            }
        }

        /// <summary>
        /// Consente alla pagina di disegnarsi.
        /// </summary>
        private void OnDraw(object sender, GameTimerEventArgs e)
        {
            if (match.gameState == GamePageState.LOST || match.gameState == GamePageState.HAVETOEXIT)
            {
                silverlightRenderer.Render();
                SharedGraphicsDeviceManager.Current.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                SharedGraphicsDeviceManager.Current.GraphicsDevice.Clear(Color.Black);
                spriteBatch.Begin();
                spriteBatch.Draw(silverlightRenderer.Texture, new Rectangle(0, 0, SharedGraphicsDeviceManager.Current.GraphicsDevice.Viewport.Width, SharedGraphicsDeviceManager.Current.GraphicsDevice.Viewport.Height), Color.White);
                spriteBatch.End();
            }
            else
            {

                silverlightRenderer.Render();
                SharedGraphicsDeviceManager.Current.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                SharedGraphicsDeviceManager.Current.GraphicsDevice.Clear(Color.Black);

                /*
                if (debug)
                    DebugShapeRenderer.Draw(new GameTime(e.TotalTime, e.ElapsedTime), Matrix.CreateLookAt(new Vector3(cameraPos.X + cameraShake.X, cameraPos.Y + cameraShake.Y, cameraPos.Z), cameraLook, Vector3.Up), Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f) + speedCurve.Evaluate(speedTime) / 1.5f, SharedGraphicsDeviceManager.Current.GraphicsDevice.Viewport.AspectRatio, 0.1f, 80.0f));
                */

                //Draw modelli 3D
                foreach (Section section in match.sections)
                {
                    foreach (ScenicElement scenic in section.ScenicElements)
                        DrawModel(section, models[scenic.model], scenic.Position, scenic.Rotation, Vector3.One);

                    foreach (Enemy enemy in section.Enemies)
                        if (enemy.IsAlive)
                        {
                            DrawModel(section, models[enemy.Model], enemy.Position, Vector3.Zero, Vector3.One);
                            DrawModel(section, models[4], new Vector3(enemy.Position.X, 0.1f, enemy.Position.Z), Vector3.Zero, new Vector3((float)(1 - enemy.Position.Y / 5), 1, (float)(1 - enemy.Position.Y / 5)));
                        }

                    foreach (PowerUp power in section.PowerUps)
                        if (power.isAvailable)
                        {
                            DrawModel(section, models[power.Model], power.Position, power.Rotation, Vector3.One);
                            DrawModel(section, models[4], new Vector3(power.Position.X, 0, power.Position.Z), Vector3.Zero, new Vector3((float)(1 - power.Position.Y / 5), 1, (float)(1 - power.Position.Y / 5)));
                        }
                }

                //Draw flash per boost
                if (match.flashCounter > 0)
                {
                    spriteBatch.Begin(SpriteSortMode.Immediate, brightnessBlend);
                    spriteBatch.Draw(whiteTexture, new Rectangle(0, 0, SharedGraphicsDeviceManager.Current.GraphicsDevice.Viewport.Width, SharedGraphicsDeviceManager.Current.GraphicsDevice.Viewport.Height), new Color(brightness, brightness, brightness, 255));
                    spriteBatch.End();

                    spriteBatch.Begin(SpriteSortMode.Immediate, contrastBlend);
                    int temp = (int)boostFlashCurve.Evaluate(match.flashCounter);
                    spriteBatch.Draw(whiteTexture, new Rectangle(0, 0, SharedGraphicsDeviceManager.Current.GraphicsDevice.Viewport.Width, SharedGraphicsDeviceManager.Current.GraphicsDevice.Viewport.Height), new Color(temp, temp, temp, 255));
                    spriteBatch.End();
                    SharedGraphicsDeviceManager.Current.GraphicsDevice.BlendState = BlendState.Opaque;
                }

                //Draw GUI 2D
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                int counter;
                for (counter = 0; counter < maxLife && match.lifepositions[counter] != heart.Height; counter++)
                {
                    Vector2 position = new Vector2(SharedGraphicsDeviceManager.Current.GraphicsDevice.Viewport.Width / 2 - barra.Width / 2 + 10 + (counter * 35), SharedGraphicsDeviceManager.Current.GraphicsDevice.Viewport.Height - heart.Height + match.lifepositions[counter]);
                    if (match.life <= 3)
                    {
                        position.X += UtilityClass.nextInt(-3, 3);
                        position.Y += UtilityClass.nextInt(-3, 3);
                    }
                    spriteBatch.Draw(heart, position, Color.White);
                }

                string tempo = match.matchTime.Hours + ":" + match.matchTime.Minutes + ":" + match.matchTime.Seconds + ":" + match.matchTime.Milliseconds;
                Vector2 timeBackPos = new Vector2(SharedGraphicsDeviceManager.Current.GraphicsDevice.Viewport.Width / 2 + barraBack.Width / 2 - timeBack.Width - 10, SharedGraphicsDeviceManager.Current.GraphicsDevice.Viewport.Height - timeBack.Height);
                spriteBatch.Draw(timeBack, timeBackPos, Color.White);

                spriteBatch.DrawString(font, tempo, new Vector2((timeBackPos.X + timeBack.Width) - (timeBack.Width / 2) - (font.MeasureString(tempo).X / 2), timeBackPos.Y), Color.Black);
                spriteBatch.Draw(barraBack, new Rectangle(SharedGraphicsDeviceManager.Current.GraphicsDevice.Viewport.Width / 2 - barraBack.Width / 2, SharedGraphicsDeviceManager.Current.GraphicsDevice.Viewport.Height - barraBack.Height, barraBack.Width, barraBack.Height), Color.White);
                spriteBatch.Draw(barra, new Rectangle(SharedGraphicsDeviceManager.Current.GraphicsDevice.Viewport.Width / 2 - barra.Width / 2, SharedGraphicsDeviceManager.Current.GraphicsDevice.Viewport.Height - barra.Height, (int)(speedCurve.Evaluate(match.speedTime) * barra.Width / Constants.MAX_SPEED), barra.Height), new Rectangle(0, 0, (int)(speedCurve.Evaluate(match.speedTime) * barra.Width / Constants.MAX_SPEED), barra.Height), Color.White);

                if (match.gameState == GamePageState.LOSING)
                    spriteBatch.Draw(blackTexture, new Rectangle(0, 0, SharedGraphicsDeviceManager.Current.GraphicsDevice.Viewport.Width, SharedGraphicsDeviceManager.Current.GraphicsDevice.Viewport.Height), Color.White * match.fadeIndex);

                spriteBatch.Draw(silverlightRenderer.Texture, Vector2.Zero, Color.White);
                spriteBatch.End();
                AdComponent.Current.Draw();
            }
        }

        /// <summary>
        /// Metodo di supporto per il draw di un modello 3D.
        /// </summary>
        /// <param name="section">Sezione del gioco su cui si trova il modello.</param>
        /// <param name="m">Modello da renderizzare.</param>
        /// <param name="position">Posizione del modello.</param>
        /// <param name="rotation">Rotazione del modello.</param>
        /// <param name="scale">Scala del modello.</param>
        private void DrawModel(Section section, Model m, Vector3 position, Vector3 rotation, Vector3 scale)
        {
            Matrix[] transforms = new Matrix[m.Bones.Count];
            m.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in m.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.FogEnabled = true;
                    effect.FogColor = new Vector3(0, 0, 0);
                    effect.FogStart = 60;
                    effect.FogEnd = 100;
                    //effect.PreferPerPixelLighting = true; ti piacerebbe...
                    effect.EnableDefaultLighting();
                    effect.View = Matrix.CreateLookAt(new Vector3(match.cameraPos.X + match.cameraShake.X, match.cameraPos.Y + match.cameraShake.Y, match.cameraPos.Z), match.cameraLook, Vector3.Up);
                    effect.World = transforms[mesh.ParentBone.Index] * Matrix.CreateScale(scale) * Matrix.CreateFromYawPitchRoll(rotation.Y, rotation.X, rotation.Z) * Matrix.CreateTranslation(new Vector3(position.X, position.Y, section.zPosition - position.Z));
                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f) + speedCurve.Evaluate(match.speedTime) / 1.5f, SharedGraphicsDeviceManager.Current.GraphicsDevice.Viewport.AspectRatio, 0.1f, 100.0f);
                }
                mesh.Draw();
            }
        }
        /*
        private void debugButton_Click(object sender, RoutedEventArgs e)
        {
            debug = !debug;
        }*/

        private void GameOverBackButton_Click(object sender, RoutedEventArgs e)
        {
            this.GoBackGameOverStoryBoard.Begin();
        }

        private void GameOverNameTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
                GameOverBackButton.Focus();
        }
    }

    /// <summary>
    /// Enum indicante lo stato in cui attualmente si trova la pagina.
    /// </summary>
    public enum GamePageState
    {
        /// <summary>
        /// Normale condizione di gioco. Draw.
        /// </summary>
        PLAY,

        /// <summary>
        /// Condizione di pausa. Draw.
        /// </summary>
        PAUSE,

        /// <summary>
        /// Si ha appena perso. Fase di fade a schermo nero per la presentazione delle statistiche del match. Draw + fade.
        /// </summary>
        LOSING,

        /// <summary>
        /// Si ha perso, il fade è concluso, si visualizzano le statistiche.
        /// </summary>
        LOST,

        /// <summary>
        /// Hai premuto invio o back nella visione delle statistiche. StoryBoard e poi back a menù.
        /// </summary>
        HAVETOEXIT
    }
}
