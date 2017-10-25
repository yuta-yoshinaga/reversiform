////////////////////////////////////////////////////////////////////////////////
///	@file			FormMain.cs
///	@brief			FormMainクラス
///	@author			Yuta Yoshinaga
///	@date			2017.10.20
///	$Version:		$
///	$Revision:		$
///
/// Copyright (c) 2017 Yuta Yoshinaga. All Rights reserved.
///
/// - 本ソフトウェアの一部又は全てを無断で複写複製（コピー）することは、
///   著作権侵害にあたりますので、これを禁止します。
/// - 本製品の使用に起因する侵害または特許権その他権利の侵害に関しては
///   当社は一切その責任を負いません。
///
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace ReversiForm
{
	////////////////////////////////////////////////////////////////////////////////
	///	@class		FormMain
	///	@brief		FormMainクラス
	///
	////////////////////////////////////////////////////////////////////////////////
	public partial class Reversi : Form
	{
		public ReversiSetting m_AppSettings;								//!< アプリ設定
		public ReversiPlay m_ReversiPlay;									//!< リバーシ本体

		////////////////////////////////////////////////////////////////////////////////
		///	@brief			コンストラクタ
		///	@fn				FormMain()
		///	@return			ありません
		///	@author			Yuta Yoshinaga
		///	@date			2017.10.20
		///
		////////////////////////////////////////////////////////////////////////////////
		public Reversi()
		{
			InitializeComponent();

			Assembly myAssembly = Assembly.GetEntryAssembly();
			string setPath = System.IO.Path.GetDirectoryName(myAssembly.Location) + "\\" + "AppSetting.xml";
			try
			{
				this.m_AppSettings = this.LoadSettingXml(setPath);
				if(this.m_AppSettings == null)
				{
					this.m_AppSettings = new ReversiSetting();
					this.SaveSettingXml(setPath,ref this.m_AppSettings);
				}
				this.m_ReversiPlay = new ReversiPlay();
				this.m_ReversiPlay.mSetting = this.m_AppSettings;
				this.m_ReversiPlay.viewMsgDlg = this.ViewMsgDlg;
				this.m_ReversiPlay.drawSingle = this.DrawSingle;
				this.m_ReversiPlay.curColMsg = this.CurColMsg;
				this.m_ReversiPlay.curStsMsg = this.CurStsMsg;
				this.m_ReversiPlay.reset();
			}
			catch (Exception ex)
			{
				System.Console.WriteLine("FormMain(1) : " + ex.Message);
				System.Console.WriteLine("FormMain(1) : " + ex.StackTrace);
			}
		}

		////////////////////////////////////////////////////////////////////////////////
		///	@brief			設定XMLファイルロード
		///	@fn				ReversiSetting LoadSettingXml(string path)
		///	@param[in]		string path		設定XMLファイルパス
		///	@return			ReversiSettingオブジェクトインスタンス
		///	@author			Yuta Yoshinaga
		///	@date			2017.10.20
		///
		////////////////////////////////////////////////////////////////////////////////
		public ReversiSetting LoadSettingXml(string path)
		{
			ReversiSetting ret = null;
			try
			{
				// *** XMLをReversiSettingオブジェクトに読み込む *** //
				XmlSerializer serializer = new XmlSerializer(typeof(ReversiSetting));
				ret = new ReversiSetting();

				FileStream fsr = new FileStream(path, FileMode.Open);

				// *** XMLファイルを読み込み、逆シリアル化（復元）する *** //
				ret = (ReversiSetting)serializer.Deserialize(fsr);
				fsr.Close();
			}
			catch (Exception ex)
			{
				System.Console.WriteLine("LoadSettingXml() : " + ex.Message);
				System.Console.WriteLine("LoadSettingXml() : " + ex.StackTrace);
				ret = null;
			}
			return ret;
		}

		////////////////////////////////////////////////////////////////////////////////
		///	@brief			設定XMLファイルセーブ
		///	@fn				int SaveSettingXml(string path,ReversiSetting appSet)
		///	@param[in]		string path			設定XMLファイルパス
		///	@param[out]		ReversiSetting appSet	設定XMLファイルオブジェクト
		///	@return			0 : 成功 それ以外 : 失敗
		///	@author			Yuta Yoshinaga
		///	@date			2017.10.20
		///
		////////////////////////////////////////////////////////////////////////////////
		public int SaveSettingXml(string path,ref ReversiSetting appSet)
		{
			int ret = 0;
			try
			{
				// *** XMLをReversiSettingオブジェクトに読み込む *** //
				XmlSerializer serializer = new XmlSerializer(typeof(ReversiSetting));

				// *** カレントディレクトリに"AppSetting.xml"というファイルで書き出す *** //
				FileStream fsw = new FileStream(path, FileMode.Create);

				// *** オブジェクトをシリアル化してXMLファイルに書き込む *** //
				serializer.Serialize(fsw, appSet);
				fsw.Close();
			}
			catch (Exception exl)
			{
				System.Console.WriteLine("SaveSettingXml() : " + exl.Message);
				System.Console.WriteLine("SaveSettingXml() : " + exl.StackTrace);
				ret = -1;
			}
			return ret;
		}

		////////////////////////////////////////////////////////////////////////////////
		///	@brief			メッセージダイアログ
		///	@fn				void ViewMsgDlg(string title , string msg)
		///	@param[in]		string title	タイトル
		///	@param[in]		string msg		メッセージ
		///	@return			ありません
		///	@author			Yuta Yoshinaga
		///	@date			2017.10.20
		///
		////////////////////////////////////////////////////////////////////////////////
		public void ViewMsgDlg(string title , string msg)
		{
			MessageBox.Show(msg, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		////////////////////////////////////////////////////////////////////////////////
		///	@brief			1マス描画
		///	@fn				void DrawSingle(int y, int x, int sts, int bk, string text)
		///	@param[in]		int y		Y座標
		///	@param[in]		int x		X座標
		///	@param[in]		int sts		ステータス
		///	@param[in]		int bk		背景
		///	@param[in]		string text	テキスト
		///	@return			ありません
		///	@author			Yuta Yoshinaga
		///	@date			2017.10.20
		///
		////////////////////////////////////////////////////////////////////////////////
		public void DrawSingle(int y, int x, int sts, int bk, string text)
		{
			PictureBox curPict = (PictureBox) this.tableLayoutPanel1.GetControlFromPosition(x,y);
			if(curPict != null)
			{

				//描画先とするImageオブジェクトを作成する
				Bitmap canvas = new Bitmap(curPict.Width, curPict.Height);
				//ImageオブジェクトのGraphicsオブジェクトを作成する
				Graphics g = Graphics.FromImage(canvas);
				Pen curPen1 = new Pen(Color.Green,4);
                //Brushオブジェクトの作成
                SolidBrush curBru1 = null;
                SolidBrush curBru2 = null;

			    if (bk == 1) {
					// *** cell_back_blue *** //
					curBru1 = new SolidBrush(Color.FromArgb(100, 0, 128, 255));
			    } else if (bk == 2) {
					// *** cell_back_red *** //
					curBru1 = new SolidBrush(Color.FromArgb(100, 255, 128, 128));
			    } else if (bk == 3) {
					// *** cell_back_magenta *** //
					curBru1 = new SolidBrush(Color.FromArgb(100, 255, 0, 255));
			    } else {
					// *** cell_back_green *** //
					curBru1 = new SolidBrush(Color.FromArgb(100, 0, 255, 0));
			    }

				if (sts == ReversiConst.REVERSI_STS_NONE)
				{
				}
				else if (sts == ReversiConst.REVERSI_STS_BLACK)
				{
					curBru2 = new SolidBrush(Color.FromArgb(255, 0, 0, 0));
				}
				else if (sts == ReversiConst.REVERSI_STS_WHITE)
				{
					curBru2 = new SolidBrush(Color.FromArgb(255, 255, 255, 255));
				}

				//位置(x, y)にWidth x Heightの四角を描く
				g.FillRectangle(curBru1, 0, 0, curPict.Width, curPict.Height);
				//位置(x, y)にWidth x Heightの四角を描く
				g.DrawRectangle(curPen1, 0, 0, curPict.Width, curPict.Height);
				//先に描いた四角に内接する楕円を黒で描く
				if(curBru2 != null) g.FillEllipse(curBru2, 0, 0, curPict.Width, curPict.Height);
				//リソースを解放する
				g.Dispose();
				//curPictに表示する
				curPict.Image = canvas;
			}
		}

		////////////////////////////////////////////////////////////////////////////////
		///	@brief			現在の色メッセージ
		///	@fn				void CurColMsg(string text)
		///	@param[in]		string text	テキスト
		///	@return			ありません
		///	@author			Yuta Yoshinaga
		///	@date			2017.10.20
		///
		////////////////////////////////////////////////////////////////////////////////
		public void CurColMsg(string text)
		{
			this.label1.Text = text;
		}

		////////////////////////////////////////////////////////////////////////////////
		///	@brief			現在のステータスメッセージ
		///	@fn				void CurStsMsg(string text)
		///	@param[in]		string text	テキスト
		///	@return			ありません
		///	@author			Yuta Yoshinaga
		///	@date			2017.10.20
		///
		////////////////////////////////////////////////////////////////////////////////
		public void CurStsMsg(string text)
		{
			this.label2.Text = text;
		}

		////////////////////////////////////////////////////////////////////////////////
		///	@brief			マスクリック
		///	@fn				void pictureBox_Click(object sender, EventArgs e)
		///	@param[in]		object sender
		///	@param[in]		EventArgs e
		///	@return			ありません
		///	@author			Yuta Yoshinaga
		///	@date			2017.10.20
		///
		////////////////////////////////////////////////////////////////////////////////
		private void pictureBox_Click(object sender, EventArgs e)
		{
			TableLayoutPanelCellPosition pos = this.tableLayoutPanel1.GetCellPosition((Control)sender);

			Console.WriteLine("click y=" + pos.Row + " x=" + pos.Column);

			this.m_ReversiPlay.reversiPlay(pos.Row,pos.Column);
		}

		////////////////////////////////////////////////////////////////////////////////
		///	@brief			リセットクリック
		///	@fn				void button1_Click(object sender, EventArgs e)
		///	@param[in]		object sender
		///	@param[in]		EventArgs e
		///	@return			ありません
		///	@author			Yuta Yoshinaga
		///	@date			2017.10.20
		///
		////////////////////////////////////////////////////////////////////////////////
		private void button1_Click(object sender, EventArgs e)
		{
			this.m_ReversiPlay.reset();
		}

		////////////////////////////////////////////////////////////////////////////////
		///	@brief			セッティングクリック
		///	@fn				void button2_Click(object sender, EventArgs e)
		///	@param[in]		object sender
		///	@param[in]		EventArgs e
		///	@return			ありません
		///	@author			Yuta Yoshinaga
		///	@date			2017.10.20
		///
		////////////////////////////////////////////////////////////////////////////////
		private void button2_Click(object sender, EventArgs e)
		{

		}
	}
}
