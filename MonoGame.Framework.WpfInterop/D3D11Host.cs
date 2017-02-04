using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MonoGame.Framework.WpfInterop
{
	/// <summary>
	/// Host a Direct3D 11 scene.
	/// </summary>
	public abstract class D3D11Host : Image, IDisposable
	{
		#region Fields

		private static readonly object _graphicsDeviceLock = new object();

		private bool _isRendering;

		// The Direct3D 11 device (shared by all D3D11Host elements):
		private static GraphicsDevice _graphicsDevice;
		private static bool? _isInDesignMode;
		private static int _referenceCount;

		private D3D11Image _d3D11Image;
		private bool _disposed;
		private TimeSpan _lastRenderingTime;
		private bool _loaded;

		// Image source:
		private RenderTarget2D _renderTarget;
		private bool _resetBackBuffer;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="D3D11Host"/> class.
		/// </summary>
		protected D3D11Host()
		{
			// defaulting to fill as that's what's needed in most cases
			Stretch = Stretch.Fill;
			
			Loaded += OnLoaded;
			Unloaded += OnUnloaded;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Determines whether the game runs in fixed timestep or not.
		/// The current implementation always calls Update and Draw after each other continuously.
		/// Since the rendering is based on the WPF render thread the exact times at which it will be called cannot be guaranteed.
		/// Therefore this value is always false.
		/// </summary>
		public bool IsFixedTimeStep => false;

		/// <summary>
		/// Gets or sets the target time between two updates. Defaults to 60fps.
		/// WPF is limiting its rendering to 60 FPS max, therefore setting a target value higher than 60 fps (lower than TimeSpan.FromSeconds(1 / 60.0)) will have no effect.
		/// </summary>
		public TimeSpan TargetElapsedTime { get; set; } = TimeSpan.FromTicks(160000); // 60 fps

		/// <summary>
		/// Gets a value indicating whether the controls runs in the context of a designer (e.g.
		/// Visual Studio Designer or Expression Blend).
		/// </summary>
		/// <value>
		/// <see langword="true" /> if controls run in design mode; otherwise, 
		/// <see langword="false" />.
		/// </value>
		public static bool IsInDesignMode
		{
			get
			{
				if (!_isInDesignMode.HasValue)
					_isInDesignMode = (bool)DependencyPropertyDescriptor.FromProperty(DesignerProperties.IsInDesignModeProperty, typeof(FrameworkElement)).Metadata.DefaultValue;

				return _isInDesignMode.Value;
			}
		}

		/// <summary>
		/// Gets the graphics device.
		/// </summary>
		/// <value>The graphics device.</value>
		public GraphicsDevice GraphicsDevice => _graphicsDevice;

		/// <summary>
		/// Default services collection.
		/// </summary>
		public GameServiceContainer Services { get; } = new GameServiceContainer();

		#endregion

		#region Methods

		public void Dispose()
		{
			if (_disposed)
				return;
			_disposed = true;

			Dispose(true);
		}

		protected abstract void Dispose(bool disposing);

		protected virtual void Initialize()
		{
		}

		/// <summary>
		/// Raises the <see cref="FrameworkElement.SizeChanged" /> event, using the specified 
		/// information as part of the eventual event data.
		/// </summary>
		/// <param name="sizeInfo">Details of the old and new size involved in the change.</param>
		protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
		{
			_resetBackBuffer = true;
			base.OnRenderSizeChanged(sizeInfo);
		}

		protected virtual void Render(GameTime time)
		{
		}

		private static void InitializeGraphicsDevice()
		{
			lock (_graphicsDeviceLock)
			{
				_referenceCount++;
				if (_referenceCount == 1)
				{
					// Create Direct3D 11 device.
					var presentationParameters = new PresentationParameters
					{
						// Do not associate graphics device with window.
						DeviceWindowHandle = IntPtr.Zero,
					};
					_graphicsDevice = new GraphicsDevice(GraphicsAdapter.DefaultAdapter, GraphicsProfile.HiDef, presentationParameters);
				}
			}
		}

		private static void UninitializeGraphicsDevice()
		{
			lock (_graphicsDeviceLock)
			{
				_referenceCount--;
				if (_referenceCount == 0)
				{
					_graphicsDevice.Dispose();
					_graphicsDevice = null;
				}
			}
		}

		private void CreateBackBuffer()
		{
			_d3D11Image.SetBackBuffer(null);
			if (_renderTarget != null)
			{
				_renderTarget.Dispose();
				_renderTarget = null;
			}

			int width = Math.Max((int)ActualWidth, 1);
			int height = Math.Max((int)ActualHeight, 1);
			_renderTarget = new RenderTarget2D(_graphicsDevice, width, height, false, SurfaceFormat.Bgr32, DepthFormat.Depth24Stencil8, 0, RenderTargetUsage.DiscardContents, true);
			_d3D11Image.SetBackBuffer(_renderTarget);
		}

		private void InitializeImageSource()
		{
			_d3D11Image = new D3D11Image();
			_d3D11Image.IsFrontBufferAvailableChanged += OnIsFrontBufferAvailableChanged;
			CreateBackBuffer();
			Source = _d3D11Image;
		}

		private void OnIsFrontBufferAvailableChanged(object sender, DependencyPropertyChangedEventArgs eventArgs)
		{
			if (_d3D11Image.IsFrontBufferAvailable)
			{
				StartRendering();
				_resetBackBuffer = true;
			}
			else
			{
				StopRendering();
			}
		}

		private void OnLoaded(object sender, RoutedEventArgs eventArgs)
		{
			if (IsInDesignMode || _loaded)
				return;

			_loaded = true;
			InitializeGraphicsDevice();
			InitializeImageSource();

			// workaround for exceptions in Onloaded being swallowed by default on x64
			// https://stackoverflow.com/questions/4807122/wpf-showdialog-swallowing-exceptions-during-window-load
			try
			{
				// initialize runs in userland
				// user can fuck up anything an have an exception thrown
				// if it throws, we don't want rendering to start
				Initialize();
				StartRendering();
			}
			catch (Exception ex)
			{
				if (Environment.Is64BitOperatingSystem || Environment.Is64BitProcess)
				{
					// catch and rethrow because WPF just swallows it silently on x64..
					BackgroundWorker deCancerifyWpf = new BackgroundWorker();
					deCancerifyWpf.DoWork += (e, arg) => { arg.Result = arg.Argument; };
					deCancerifyWpf.RunWorkerCompleted += (e, arg) =>
					{
						// who needs a proper stacktrace anyway
						// at least we get to see the exception..
						throw new Exception("Initialize failed, see inner exception for details.", (Exception)arg.Result);
					};
					deCancerifyWpf.RunWorkerAsync(ex);
				}
			}
		}

		private void OnRendering(object sender, EventArgs eventArgs)
		{
			if (!_isRendering)
				return;

			// Recreate back buffer if necessary.
			if (_resetBackBuffer)
				CreateBackBuffer();

			// CompositionTarget.Rendering event may be raised multiple times per frame
			// (e.g. during window resizing).
			// this will be apparent when the last rendering time equals the new argument
			var renderingEventArgs = (RenderingEventArgs)eventArgs;
			if (_lastRenderingTime != renderingEventArgs.RenderingTime)
			{
				// get time since last actual rendering

				var deltaTicks = renderingEventArgs.RenderingTime.Ticks - _lastRenderingTime.Ticks;
				var delta = TimeSpan.FromTicks(deltaTicks);
				// accumulate until time is greater than target time between frames
				if (delta >= TargetElapsedTime)
				{
					// enough time has passed to draw a single frame

					GraphicsDevice.SetRenderTarget(_renderTarget);
					Render(new GameTime(renderingEventArgs.RenderingTime, delta));
					GraphicsDevice.Flush();

					_lastRenderingTime = renderingEventArgs.RenderingTime;
				}
			}
			else if (_resetBackBuffer)
			{
				// always force render when backbuffer is reset (happens during resize due to size change)
				// if we don't always render it will remain black until next frame is drawn
				GraphicsDevice.SetRenderTarget(_renderTarget);
				Render(new GameTime(renderingEventArgs.RenderingTime, TimeSpan.Zero));
				GraphicsDevice.Flush();
			}

			_d3D11Image.Invalidate(); // Always invalidate D3DImage to reduce flickering
									  // during window resizing.

			_resetBackBuffer = false;
		}
		
		private void OnUnloaded(object sender, RoutedEventArgs eventArgs)
		{
			if (IsInDesignMode)
				return;

			StopRendering();
			Dispose();
			UnitializeImageSource();
			UninitializeGraphicsDevice();
		}

		private void StartRendering()
		{
			if (_isRendering)
				return;

			CompositionTarget.Rendering += OnRendering;
			_isRendering = true;
		}

		private void StopRendering()
		{
			if (!_isRendering)
				return;

			CompositionTarget.Rendering -= OnRendering;
			_isRendering = false;
		}

		private void UnitializeImageSource()
		{
			_d3D11Image.IsFrontBufferAvailableChanged -= OnIsFrontBufferAvailableChanged;
			Source = null;

			if (_d3D11Image != null)
			{
				_d3D11Image.Dispose();
				_d3D11Image = null;
			}
			if (_renderTarget != null)
			{
				_renderTarget.Dispose();
				_renderTarget = null;
			}
		}

		#endregion
	}
}