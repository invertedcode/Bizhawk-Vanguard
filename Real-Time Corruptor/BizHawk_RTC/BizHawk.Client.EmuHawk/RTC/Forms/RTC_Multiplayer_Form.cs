﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Windows.Forms;
using Ceras;
using RTC.Legacy;

namespace RTC
{
	public partial class RTC_Multiplayer_Form : Form, IAutoColorize
	{
		Timer streamTimer = null;

		public int GameOfSwapCounter
		{
			get
			{
				return _GameOfSwapCounter;
			}
			set
			{
				_GameOfSwapCounter = value;

				if (_GameOfSwapCounter >= 0)
					UpdateRedBar(_GameOfSwapCounter * 4);
				else
					UpdateRedBar(0);
			}
		}

		int _GameOfSwapCounter = 0;
		bool GameOfSwapHost = false;
		public Timer GameOfSwapTimer = null;

		int fps = 30;

		public RTC_Multiplayer_Form()
		{
			InitializeComponent();
		}

		private void RTC_Multi_Form_Load(object sender, EventArgs e)
		{
			//assing event handlers here

			RTC_NetcoreImplementation.Multiplayer = new RTC_NetCore();

			RTC_NetcoreImplementation.Multiplayer.ClientConnecting += Multiplayer_ClientConnecting;
			RTC_NetcoreImplementation.Multiplayer.ClientConnected += Multiplayer_ClientConnected;
			RTC_NetcoreImplementation.Multiplayer.ClientDisconnected += Multiplayer_ClientDisconnected;
			RTC_NetcoreImplementation.Multiplayer.ClientConnectionLost += Multiplayer_ClientConnectionLost;
			RTC_NetcoreImplementation.Multiplayer.ClientReconnecting += Multiplayer_ClientReconnecting;

			RTC_NetcoreImplementation.Multiplayer.ServerStarted += Multiplayer_ServerStarted;
			RTC_NetcoreImplementation.Multiplayer.ServerConnected += Multiplayer_ServerConnected;
			RTC_NetcoreImplementation.Multiplayer.ServerDisconnected += Multiplayer_ServerDisconnected;
			RTC_NetcoreImplementation.Multiplayer.ServerConnectionLost += Multiplayer_ServerConnectionLost;

			cbStreamFps.SelectedIndex = 2;

			string[] cheekyHeadlines =
			{
				"Yes, it kinda works™",
				"Yes, it kinda works™",
				"Yes, it kinda works™",
				"Yes, it kinda works™",
				"Yes, it kinda works™",
				"Yes, it kinda works™",
				"Yes, it kinda works™",
				"Yes, it kinda works™",
				"Yes, it kinda works™",
				"Yes, it kinda works™",
				"Yes, it kinda works™",
				"Yes, it kinda works™",
				"Yes, it kinda works™",
				"Yes, it kinda works™",
				"Yes, it kinda works™",
				"Yes, it kinda works™",
				"Some sort of netplay",
			};

			lbCheekyHeadline.Text = cheekyHeadlines[RTC_Corruptcore.RND.Next(cheekyHeadlines.Length)];
		}

		private void Multiplayer_ServerConnectionLost(object sender, EventArgs e)
		{
			lbServerStatus.Text = "Server Status : friend has dropped";
		}

		private void Multiplayer_ServerDisconnected(object sender, EventArgs e)
		{
			tbClientAdress.Visible = true;
			btnStartClient.Visible = true;
			lbClientStatus.Visible = true;
			lbClient.Visible = true;

			lbServerStatus.Text = "Server Status : pre-heated";

			btnStartServer.Text = "Start Server";
		}

		private void Multiplayer_ServerConnected(object sender, EventArgs e)
		{
			lbServerStatus.Text = "Server Status : Someone said hi";
		}

		private void Multiplayer_ServerStarted(object sender, EventArgs e)
		{
			tbClientAdress.Visible = false;
			btnStartClient.Visible = false;
			lbClientStatus.Visible = false;
			lbClient.Visible = false;

			btnStartServer.Text = "Stop Server";

			if (!lbServerStatus.Text.Contains("dropped"))
				lbServerStatus.Text = "Server Status : the thing started or something";
		}

		private void Multiplayer_ClientReconnecting(object sender, EventArgs e)
		{
			btnStartClient.Text = $"Reconnecting";
		}

		private void Multiplayer_ClientConnectionLost(object sender, EventArgs e)
		{
			lbClientStatus.Text = "Client Status : very sad (connection lost)";
		}

		private void Multiplayer_ClientDisconnected(object sender, EventArgs e)
		{
			btnStartServer.Visible = true;
			lbServerStatus.Visible = true;
			lbServer.Visible = true;

			tbClientAdress.Enabled = true;
			lbClientStatus.Text = "Client Status : sad";

			btnStartClient.Text = "Connect to server";
		}

		private void Multiplayer_ClientConnected(object sender, EventArgs e)
		{
			btnStartClient.Text = "Disconnect";
			lbClientStatus.Text = "Client Status : actually looks kind of connected";
		}

		private void Multiplayer_ClientConnecting(object sender, EventArgs e)
		{
			btnStartServer.Visible = false;
			lbServerStatus.Visible = false;
			lbServer.Visible = false;

			btnStartClient.Text = "Connecting";
			tbClientAdress.Enabled = false;
			lbClientStatus.Text = "Client Status : Trying to spot the server from afar";
		}

		private void btnStartClient_Click(object sender, EventArgs e)
		{
			if (btnStartClient.Text == "Disconnect" || btnStartClient.Text == "Reconnecting" || btnStartClient.Text == "Connecting")
			{
				RTC_NetcoreImplementation.Multiplayer.StopNetworking();
				return;
			}

			RTC_NetcoreImplementation.Multiplayer.StartNetworking(NetworkSide.CLIENT);
		}

		private void btnStartServer_Click(object sender, EventArgs e)
		{
			if (btnStartServer.Text == "Stop Server")
			{
				RTC_NetcoreImplementation.Multiplayer.StopNetworking();
				return;
			}

			RTC_NetcoreImplementation.Multiplayer.StartNetworking(NetworkSide.SERVER);
		}

		private void btnPushBlastToServer_Click(object sender, EventArgs e)
		{
			RTC_NetcoreImplementation.Multiplayer.SendBlastlayer();
		}

		private void tbShowIp_TextChanged(object sender, EventArgs e)
		{
			tbShowIp.Text = new WebClient().DownloadString("http://icanhazip.com");
		}

		private void btnPullGameFromServer_Click(object sender, EventArgs e)
		{
			RTC_NetcoreImplementation.Multiplayer.SendCommand(new RTC_Command(CommandType.PULLROM), false);
		}

		private void btnPushGameToServer_Click(object sender, EventArgs e)
		{
			RTC_NetcoreImplementation.Multiplayer.SendCommand(new RTC_Command(CommandType.PULLROM), true);
		}

		private void btnPullStateFromServer_Click(object sender, EventArgs e)
		{
			RTC_NetcoreImplementation.Multiplayer.SendCommand(new RTC_Command(CommandType.PULLSTATE), false);
		}

		private void btnPushStateToServer_Click(object sender, EventArgs e)
		{
			RTC_NetcoreImplementation.Multiplayer.SendCommand(new RTC_Command(CommandType.PULLSTATE), true);
		}

		private void tbServerPort_TextChanged(object sender, EventArgs e)
		{
			if (RTC_NetcoreImplementation.Multiplayer.side == NetworkSide.DISCONNECTED)
				RTC_NetcoreImplementation.Multiplayer.port = Convert.ToInt32(tbServerPort.Text);
			else
				tbServerPort.Text = RTC_NetcoreImplementation.Multiplayer.port.ToString();
		}

		private void btnSwapGameState_Click(object sender, EventArgs e)
		{
			RTC_NetcoreImplementation.Multiplayer.SwapGameState();
		}

		private void btnPushScreenToPear_Click(object sender, EventArgs e)
		{
			RTC_NetcoreImplementation.Multiplayer.SendCommand(new RTC_Command(CommandType.PULLSCREEN), true);
		}

		private void btnPullScreenToPear_Click(object sender, EventArgs e)
		{
			RTC_NetcoreImplementation.Multiplayer.SendCommand(new RTC_Command(CommandType.PULLSCREEN), false);
		}

		public void cbStreamScreenToPeer_CheckedChanged(object sender, EventArgs e)
		{
			if (cbStreamScreenToPeer.Checked)
			{
				streamTimer = new Timer
				{
					Interval = (1000 / fps)
				};
				streamTimer.Tick += StreamTimer_Tick;
				streamTimer.Start();
			}
			else
			{
				streamTimer.Stop();
				streamTimer = null;
			}
		}

		private void StreamTimer_Tick(object sender, EventArgs e)
		{
			if (RTC_Hooks.BIZHAWK_ISMAINFORMVISIBLE())
			{
				RTC_Command cmdBack = new RTC_Command(CommandType.PUSHSCREEN);

				Bitmap bmp = RTC_Hooks.BIZHAWK_GET_SCREENSHOT();

				if (cbCompressStream.Checked)
					cmdBack.ScreenArr = RTC_Extensions.ImageToByteArray(SaveJPG100(bmp, 60), ImageFormat.Jpeg);
				else
					cmdBack.ScreenArr = RTC_Extensions.ImageToByteArray(SaveJPG100(bmp, 60), ImageFormat.Bmp);
				RTC_NetcoreImplementation.Multiplayer.PeerCommandQueue.AddLast(cmdBack);
			}
		}

		public static void SaveJPG100(Bitmap bmp, Stream stream)
		{
			EncoderParameters encoderParameters = new EncoderParameters(1);
			encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 100L);
			bmp.Save(stream, GetEncoder(ImageFormat.Jpeg), encoderParameters);
		}

		public static Image SaveJPG100(Bitmap bmp, long quality)
		{
			MemoryStream stream = new MemoryStream();
			EncoderParameters encoderParameters = new EncoderParameters(1);
			encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, quality);
			bmp.Save(stream, GetEncoder(ImageFormat.Jpeg), encoderParameters);

			return Image.FromStream(stream);
		}

		public static ImageCodecInfo GetEncoder(ImageFormat format)
		{
			ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

			foreach (ImageCodecInfo codec in codecs)
			{
				if (codec.FormatID == format.Guid)
				{
					return codec;
				}
			}

			return null;
		}

		public void SetStreamingFPS(int _fps)
		{
			fps = _fps;

			if (streamTimer != null)
			{
				streamTimer.Interval = (1000 / fps);
			}
		}

		private void rbStream30fps_CheckedChanged(object sender, EventArgs e)
		{
			SetStreamingFPS(30);
		}

		private void rbStream20fps_CheckedChanged(object sender, EventArgs e)
		{
			SetStreamingFPS(20);
		}

		private void rbStream10fps_CheckedChanged(object sender, EventArgs e)
		{
			SetStreamingFPS(10);
		}

		private void btnClearNetworkCache_Click(object sender, EventArgs e)
		{
			RTC_NetcoreImplementation.Multiplayer.ClearNetowrkCache();
		}

		private void btnGameOfSwap_Click(object sender, EventArgs e)
		{
			if (btnGameOfSwap.ForeColor == Color.Red)
			{
				StopGameOfSwap();
				return;
			}

			if (RTC_NetcoreImplementation.Multiplayer.side == NetworkSide.DISCONNECTED)
				return;

			StartGameOfSwap(true);
		}

		public void StopGameOfSwap(bool fromStopCommand = false)
		{
			if (GameOfSwapTimer != null)
			{
				GameOfSwapTimer.Stop();
				GameOfSwapTimer = null;
			}

			btnGameOfSwap.ForeColor = Color.White;
			GameOfSwapCounter = -1;

			if (!fromStopCommand)
				RTC_NetcoreImplementation.Multiplayer.SendCommand(new RTC_Command(CommandType.GAMEOFSWAPSTOP), false);
		}

		public void StartGameOfSwap(bool isGameHost)
		{
			GameOfSwapHost = isGameHost;
			GameOfSwapTimer = new Timer
			{
				Interval = 200
			};
			GameOfSwapTimer.Tick += GameOfSwapTimer_Tick;
			GameOfSwapTimer.Start();

			if (isGameHost)
				RTC_NetcoreImplementation.Multiplayer.SendCommand(new RTC_Command(CommandType.GAMEOFSWAPSTART), false);

			GameOfSwapCounter = 64;

			if (!cbStreamScreenToPeer.Checked)
				cbStreamScreenToPeer.Checked = true;

			btnGameOfSwap.ForeColor = Color.Red;
		}

		private void GameOfSwapTimer_Tick(object sender, EventArgs e)
		{
			GameOfSwapCounter--;

			if (GameOfSwapCounter == 0 && GameOfSwapHost)
				RTC_NetcoreImplementation.Multiplayer.SwapGameState();
		}

		private void btnPopoutPeerGameScreen_Click(object sender, EventArgs e)
		{
			S.GET<RTC_MultiPeerPopout_Form>().Show();

			pbPeerScreen.Visible = false;
			pnPeerRedBar.Visible = false;
			btnPopoutPeerGameScreen.Visible = false;
		}

		public void UpdateRedBar(int sizeX)
		{
			pnPeerRedBar.Size = new Size(sizeX, 3);
			S.GET<RTC_MultiPeerPopout_Form>().pnPeerRedBar.Size = new Size(Convert.ToInt32((Convert.ToDouble(sizeX) / 256f) * Convert.ToDouble(S.GET<RTC_MultiPeerPopout_Form>().pnPlacer.Size.Width)), 5);
		}

		private void RTC_Multi_Form_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (e.CloseReason != CloseReason.FormOwnerClosing)
			{
				e.Cancel = true;
				this.Hide();
			}
		}

		private void btnPushStashkeyToServer_Click(object sender, EventArgs e)
		{
			RTC_NetcoreImplementation.Multiplayer.SendStashkey();
		}

		private void btnRequestStream_Click(object sender, EventArgs e)
		{
			RTC_NetcoreImplementation.Multiplayer.SendCommand(new RTC_Command(CommandType.REQUESTSTREAM), false);
		}

		private void btnStartServer_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{
				Point locate = new Point((sender as Control).Location.X + (sender as Control).Parent.Location.X + e.Location.X, (sender as Control).Location.Y + (sender as Control).Parent.Location.Y + e.Location.Y);

				ContextMenuStrip columnsMenu = new ContextMenuStrip();
				(columnsMenu.Items.Add("BRUTAL DISCONNECT", null, new EventHandler((ob, ev) => { RTC_NetcoreImplementation.Multiplayer.StopNetworking(true, false); })) as ToolStripMenuItem).Enabled = RTC_NetcoreImplementation.Multiplayer.side != NetworkSide.DISCONNECTED;
				columnsMenu.Show(this, locate);
			}
		}

		public void btnSplitscreen_Click(object sender, EventArgs e)
		{
			if (btnSplitscreen.ForeColor == Color.Red)
			{
				S.GET<RTC_MultiPeerPopout_Form>().SetSplitscreen(false);
				btnSplitscreen.ForeColor = Color.White;
				return;
			}

			S.GET<RTC_MultiPeerPopout_Form>().SetSplitscreen(true);

			if (btnPopoutPeerGameScreen.Visible)
				btnPopoutPeerGameScreen_Click(null, null);

			RTC_NetcoreImplementation.Multiplayer.SendCommand(new RTC_Command(CommandType.REQUESTSTREAM), false);
			cbStreamScreenToPeer.Checked = true;
			btnSplitscreen.ForeColor = Color.Red;
		}

		private void btnBlastBoard_Click(object sender, EventArgs e)
		{
			S.GET<RTC_StockpileBlastBoard_Form>().Show();
		}

		private void tbClientAdress_TextChanged(object sender, EventArgs e)
		{
			RTC_NetcoreImplementation.Multiplayer.address = tbClientAdress.Text;
		}

		private void cbStreamFps_SelectedIndexChanged(object sender, EventArgs e)
		{
			SetStreamingFPS(Convert.ToInt32(cbStreamFps.SelectedItem.ToString().Split('f')[0]));
		}
	}

	[Serializable]
	[Ceras.MemberConfig(TargetMember.All)]
	public class RTC_Command
	{
		public CommandType Type;
		public CommandType ReturnedFrom;
		public bool Priority = false;
		public Guid? requestGuid = null;
		public object objectValue = null;

		public BlastLayer blastlayer = null;
		public bool isReplay = false;
		public byte[] romData = null;
		public string romFilename = null;
		public StashKey stashkey = null;

		public byte[] ScreenArr = null;

		public RTC_Command(CommandType _Type)
		{
			Type = _Type;
		}
		public RTC_Command()
		{
		}
	}
}
