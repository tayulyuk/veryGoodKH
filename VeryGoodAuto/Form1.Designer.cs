namespace VeryGoodAuto
{
    partial class Form1
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.buyConditionCheckBox = new System.Windows.Forms.CheckBox();
            this.buyConditionComboBox = new System.Windows.Forms.ComboBox();
            this.sellConditionComboBox = new System.Windows.Forms.ComboBox();
            this.takeProfitNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.stopLossNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.totalAmountNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown4 = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tradingStartButton = new System.Windows.Forms.Button();
            this.tradingStopButton = new System.Windows.Forms.Button();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.takeProfitCheckBox = new System.Windows.Forms.CheckBox();
            this.stopLossChheckBox = new System.Windows.Forms.CheckBox();
            this.axKHOpenAPI1 = new AxKHOpenAPILib.AxKHOpenAPI();
            this.label5 = new System.Windows.Forms.Label();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.priceManualNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.accountComboBox = new System.Windows.Forms.ComboBox();
            this.label18 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.itemNameLabel = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.userNameLabel = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.guBunComboBox = new System.Windows.Forms.ComboBox();
            this.amountManulNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.itemCodeTextBox = new System.Windows.Forms.TextBox();
            this.originOrderNumtextBox = new System.Windows.Forms.TextBox();
            this.tradeOptionComboBox = new System.Windows.Forms.ComboBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.searchButton = new System.Windows.Forms.Button();
            this.searchTextBox = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.cancelButton = new System.Windows.Forms.Button();
            this.changeButton = new System.Windows.Forms.Button();
            this.sellButton = new System.Windows.Forms.Button();
            this.buyButton = new System.Windows.Forms.Button();
            this.conditionDataGridView = new System.Windows.Forms.DataGridView();
            this.조건식_조건번호 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.조건식_조건식명 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.condigionItemDataGridView = new System.Windows.Forms.DataGridView();
            this.조건종목_종목코드 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.조건종목_종목명 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.조건종목_현재가 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.조건종목_전일대비 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.조건종목_등락율 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.조건종목_거래량 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.insertListBox = new System.Windows.Forms.ListBox();
            this.deleteListBox = new System.Windows.Forms.ListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.outStandingDataGridView = new System.Windows.Forms.DataGridView();
            this.미체결_주문번호 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.미체결_종목코드 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.미체결_종목명 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.미체결_주문수량 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.미체결_미체결수량 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.미체결_주문가격 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.미체결_현재가 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.미체결_체결가 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.미체결_주문구분 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.미체결_시간 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.realizationProfitLabel = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.당일손익율Label = new System.Windows.Forms.Label();
            this.손익율Label = new System.Windows.Forms.Label();
            this.당일손익금액Label = new System.Windows.Forms.Label();
            this.액Label = new System.Windows.Forms.Label();
            this.예탁자산평가액Label = new System.Windows.Forms.Label();
            this.산평가액Label1 = new System.Windows.Forms.Label();
            this.총매입금액Label1 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.예수금Label = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.balanceDataGridView = new System.Windows.Forms.DataGridView();
            this.잔고_종목코드 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.잔고_현재가 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.잔고_평가금액 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.잔고_종목명 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.잔고_보유수량 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.잔고_평균단가 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.잔고_손익금액 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.잔고_손익율 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.잔고_매입금액 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.takeProfitNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.stopLossNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.totalAmountNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axKHOpenAPI1)).BeginInit();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.priceManualNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.amountManulNumericUpDown)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.conditionDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.condigionItemDataGridView)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.outStandingDataGridView)).BeginInit();
            this.tableLayoutPanel4.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.balanceDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.SkyBlue;
            this.tableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.Controls.Add(this.buyConditionCheckBox, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.buyConditionComboBox, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.sellConditionComboBox, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.takeProfitNumericUpDown, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.stopLossNumericUpDown, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.totalAmountNumericUpDown, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.numericUpDown4, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.tradingStartButton, 2, 4);
            this.tableLayoutPanel1.Controls.Add(this.tradingStopButton, 3, 4);
            this.tableLayoutPanel1.Controls.Add(this.checkBox2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.takeProfitCheckBox, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.stopLossChheckBox, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.axKHOpenAPI1, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.label5, 1, 4);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(5, 4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 18F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 18F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 18F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 18F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 28F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(756, 200);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // buyConditionCheckBox
            // 
            this.buyConditionCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.buyConditionCheckBox.AutoSize = true;
            this.buyConditionCheckBox.Checked = true;
            this.buyConditionCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.buyConditionCheckBox.Location = new System.Drawing.Point(4, 10);
            this.buyConditionCheckBox.Name = "buyConditionCheckBox";
            this.buyConditionCheckBox.Size = new System.Drawing.Size(144, 16);
            this.buyConditionCheckBox.TabIndex = 0;
            this.buyConditionCheckBox.Text = "매수조건식";
            this.buyConditionCheckBox.UseVisualStyleBackColor = true;
            // 
            // buyConditionComboBox
            // 
            this.buyConditionComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.buyConditionComboBox.FormattingEnabled = true;
            this.buyConditionComboBox.Location = new System.Drawing.Point(155, 8);
            this.buyConditionComboBox.Name = "buyConditionComboBox";
            this.buyConditionComboBox.Size = new System.Drawing.Size(219, 20);
            this.buyConditionComboBox.TabIndex = 4;
            // 
            // sellConditionComboBox
            // 
            this.sellConditionComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.sellConditionComboBox.FormattingEnabled = true;
            this.sellConditionComboBox.Location = new System.Drawing.Point(155, 43);
            this.sellConditionComboBox.Name = "sellConditionComboBox";
            this.sellConditionComboBox.Size = new System.Drawing.Size(219, 20);
            this.sellConditionComboBox.TabIndex = 5;
            // 
            // takeProfitNumericUpDown
            // 
            this.takeProfitNumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.takeProfitNumericUpDown.DecimalPlaces = 1;
            this.takeProfitNumericUpDown.Location = new System.Drawing.Point(155, 77);
            this.takeProfitNumericUpDown.Name = "takeProfitNumericUpDown";
            this.takeProfitNumericUpDown.Size = new System.Drawing.Size(219, 21);
            this.takeProfitNumericUpDown.TabIndex = 6;
            // 
            // stopLossNumericUpDown
            // 
            this.stopLossNumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.stopLossNumericUpDown.DecimalPlaces = 1;
            this.stopLossNumericUpDown.Location = new System.Drawing.Point(155, 112);
            this.stopLossNumericUpDown.Maximum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.stopLossNumericUpDown.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.stopLossNumericUpDown.Name = "stopLossNumericUpDown";
            this.stopLossNumericUpDown.Size = new System.Drawing.Size(219, 21);
            this.stopLossNumericUpDown.TabIndex = 7;
            // 
            // totalAmountNumericUpDown
            // 
            this.totalAmountNumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.totalAmountNumericUpDown.Location = new System.Drawing.Point(569, 7);
            this.totalAmountNumericUpDown.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.totalAmountNumericUpDown.Name = "totalAmountNumericUpDown";
            this.totalAmountNumericUpDown.Size = new System.Drawing.Size(183, 21);
            this.totalAmountNumericUpDown.TabIndex = 8;
            // 
            // numericUpDown4
            // 
            this.numericUpDown4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDown4.Location = new System.Drawing.Point(569, 42);
            this.numericUpDown4.Name = "numericUpDown4";
            this.numericUpDown4.Size = new System.Drawing.Size(183, 21);
            this.numericUpDown4.TabIndex = 9;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(381, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(181, 12);
            this.label1.TabIndex = 10;
            this.label1.Text = "1회 매수금액";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(381, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(181, 12);
            this.label2.TabIndex = 11;
            this.label2.Text = "최대종목수";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tradingStartButton
            // 
            this.tradingStartButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.tradingStartButton.BackColor = System.Drawing.Color.Lime;
            this.tradingStartButton.Location = new System.Drawing.Point(386, 144);
            this.tradingStartButton.Name = "tradingStartButton";
            this.tradingStartButton.Size = new System.Drawing.Size(171, 52);
            this.tradingStartButton.TabIndex = 12;
            this.tradingStartButton.Text = "시작(자동)";
            this.tradingStartButton.UseVisualStyleBackColor = false;
            // 
            // tradingStopButton
            // 
            this.tradingStopButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.tradingStopButton.BackColor = System.Drawing.Color.Fuchsia;
            this.tradingStopButton.Enabled = false;
            this.tradingStopButton.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.tradingStopButton.Location = new System.Drawing.Point(578, 144);
            this.tradingStopButton.Name = "tradingStopButton";
            this.tradingStopButton.Size = new System.Drawing.Size(165, 52);
            this.tradingStopButton.TabIndex = 13;
            this.tradingStopButton.Text = "중지(자동)";
            this.tradingStopButton.UseVisualStyleBackColor = false;
            // 
            // checkBox2
            // 
            this.checkBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBox2.AutoSize = true;
            this.checkBox2.Checked = true;
            this.checkBox2.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox2.Location = new System.Drawing.Point(4, 45);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(144, 16);
            this.checkBox2.TabIndex = 14;
            this.checkBox2.Text = "매도조건식";
            this.checkBox2.UseVisualStyleBackColor = true;
            // 
            // takeProfitCheckBox
            // 
            this.takeProfitCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.takeProfitCheckBox.AutoSize = true;
            this.takeProfitCheckBox.Location = new System.Drawing.Point(4, 80);
            this.takeProfitCheckBox.Name = "takeProfitCheckBox";
            this.takeProfitCheckBox.Size = new System.Drawing.Size(144, 16);
            this.takeProfitCheckBox.TabIndex = 15;
            this.takeProfitCheckBox.Text = "수익률";
            this.takeProfitCheckBox.UseVisualStyleBackColor = true;
            // 
            // stopLossChheckBox
            // 
            this.stopLossChheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.stopLossChheckBox.AutoSize = true;
            this.stopLossChheckBox.Location = new System.Drawing.Point(4, 115);
            this.stopLossChheckBox.Name = "stopLossChheckBox";
            this.stopLossChheckBox.Size = new System.Drawing.Size(144, 16);
            this.stopLossChheckBox.TabIndex = 16;
            this.stopLossChheckBox.Text = "손절";
            this.stopLossChheckBox.UseVisualStyleBackColor = true;
            // 
            // axKHOpenAPI1
            // 
            this.axKHOpenAPI1.Enabled = true;
            this.axKHOpenAPI1.Location = new System.Drawing.Point(4, 144);
            this.axKHOpenAPI1.Name = "axKHOpenAPI1";
            this.axKHOpenAPI1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axKHOpenAPI1.OcxState")));
            this.axKHOpenAPI1.Size = new System.Drawing.Size(144, 50);
            this.axKHOpenAPI1.TabIndex = 17;
            this.axKHOpenAPI1.Visible = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.Color.Red;
            this.label5.Location = new System.Drawing.Point(155, 141);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(205, 12);
            this.label5.TabIndex = 18;
            this.label5.Text = "% 손절은 음수(-0.0)방식으로 입력 %";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel2.BackColor = System.Drawing.Color.MistyRose;
            this.tableLayoutPanel2.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 27.68924F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 31.0757F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 41.23506F));
            this.tableLayoutPanel2.Controls.Add(this.priceManualNumericUpDown, 1, 4);
            this.tableLayoutPanel2.Controls.Add(this.accountComboBox, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.label18, 0, 5);
            this.tableLayoutPanel2.Controls.Add(this.label15, 0, 4);
            this.tableLayoutPanel2.Controls.Add(this.label12, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.label9, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.itemNameLabel, 2, 1);
            this.tableLayoutPanel2.Controls.Add(this.label6, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.userNameLabel, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.label3, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.guBunComboBox, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.amountManulNumericUpDown, 1, 3);
            this.tableLayoutPanel2.Controls.Add(this.itemCodeTextBox, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.originOrderNumtextBox, 1, 5);
            this.tableLayoutPanel2.Controls.Add(this.tradeOptionComboBox, 2, 5);
            this.tableLayoutPanel2.Controls.Add(this.flowLayoutPanel1, 2, 2);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(768, 4);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 6;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(503, 170);
            this.tableLayoutPanel2.TabIndex = 1;
            // 
            // priceManualNumericUpDown
            // 
            this.priceManualNumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.priceManualNumericUpDown.Location = new System.Drawing.Point(143, 116);
            this.priceManualNumericUpDown.Maximum = new decimal(new int[] {
            1410065408,
            2,
            0,
            0});
            this.priceManualNumericUpDown.Name = "priceManualNumericUpDown";
            this.priceManualNumericUpDown.Size = new System.Drawing.Size(149, 21);
            this.priceManualNumericUpDown.TabIndex = 14;
            // 
            // accountComboBox
            // 
            this.accountComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.accountComboBox.FormattingEnabled = true;
            this.accountComboBox.Location = new System.Drawing.Point(143, 4);
            this.accountComboBox.Name = "accountComboBox";
            this.accountComboBox.Size = new System.Drawing.Size(149, 20);
            this.accountComboBox.TabIndex = 16;
            // 
            // label18
            // 
            this.label18.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(4, 149);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(132, 12);
            this.label18.TabIndex = 15;
            this.label18.Text = "원주문번호";
            this.label18.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label15
            // 
            this.label15.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(4, 120);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(132, 12);
            this.label15.TabIndex = 12;
            this.label15.Text = "주문가격";
            this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label12
            // 
            this.label12.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(4, 92);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(132, 12);
            this.label12.TabIndex = 9;
            this.label12.Text = "주문수량";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label9
            // 
            this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(4, 64);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(132, 12);
            this.label9.TabIndex = 6;
            this.label9.Text = "거래구분";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // itemNameLabel
            // 
            this.itemNameLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.itemNameLabel.AutoSize = true;
            this.itemNameLabel.Location = new System.Drawing.Point(299, 29);
            this.itemNameLabel.Name = "itemNameLabel";
            this.itemNameLabel.Size = new System.Drawing.Size(200, 27);
            this.itemNameLabel.TabIndex = 5;
            this.itemNameLabel.Text = "종목명";
            this.itemNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(4, 36);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(132, 12);
            this.label6.TabIndex = 3;
            this.label6.Text = "종목코드";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // userNameLabel
            // 
            this.userNameLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.userNameLabel.AutoSize = true;
            this.userNameLabel.Location = new System.Drawing.Point(299, 1);
            this.userNameLabel.Name = "userNameLabel";
            this.userNameLabel.Size = new System.Drawing.Size(200, 27);
            this.userNameLabel.TabIndex = 2;
            this.userNameLabel.Text = "사용자명";
            this.userNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 8);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(132, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "계좌번호";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // guBunComboBox
            // 
            this.guBunComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.guBunComboBox.FormattingEnabled = true;
            this.guBunComboBox.Items.AddRange(new object[] {
            "00 : 지정가",
            "03 : 시장가",
            "05 : 조건부지정가",
            "06 : 최유리지정가",
            "07 : 최우선지정가",
            "10 : 지정가IOC",
            "13 : 시장가IOC",
            "16 : 최유리IOC",
            "20 : 지정가FOK",
            "23 : 시장가FOK",
            "26 : 최유리FOK",
            "61 : 장전시간외종가",
            "62 : 시간외단일가매매",
            "81 : 장후시간외종가"});
            this.guBunComboBox.Location = new System.Drawing.Point(143, 60);
            this.guBunComboBox.Name = "guBunComboBox";
            this.guBunComboBox.Size = new System.Drawing.Size(149, 20);
            this.guBunComboBox.TabIndex = 17;
            this.guBunComboBox.Text = "00 : 지정가";
            // 
            // amountManulNumericUpDown
            // 
            this.amountManulNumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.amountManulNumericUpDown.Location = new System.Drawing.Point(143, 88);
            this.amountManulNumericUpDown.Maximum = new decimal(new int[] {
            1410065408,
            2,
            0,
            0});
            this.amountManulNumericUpDown.Name = "amountManulNumericUpDown";
            this.amountManulNumericUpDown.Size = new System.Drawing.Size(149, 21);
            this.amountManulNumericUpDown.TabIndex = 18;
            // 
            // itemCodeTextBox
            // 
            this.itemCodeTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.itemCodeTextBox.Location = new System.Drawing.Point(143, 32);
            this.itemCodeTextBox.Name = "itemCodeTextBox";
            this.itemCodeTextBox.Size = new System.Drawing.Size(149, 21);
            this.itemCodeTextBox.TabIndex = 20;
            // 
            // originOrderNumtextBox
            // 
            this.originOrderNumtextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.originOrderNumtextBox.Location = new System.Drawing.Point(143, 144);
            this.originOrderNumtextBox.Name = "originOrderNumtextBox";
            this.originOrderNumtextBox.Size = new System.Drawing.Size(149, 21);
            this.originOrderNumtextBox.TabIndex = 21;
            // 
            // tradeOptionComboBox
            // 
            this.tradeOptionComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tradeOptionComboBox.FormattingEnabled = true;
            this.tradeOptionComboBox.Items.AddRange(new object[] {
            "매수",
            "매도"});
            this.tradeOptionComboBox.Location = new System.Drawing.Point(299, 144);
            this.tradeOptionComboBox.Name = "tradeOptionComboBox";
            this.tradeOptionComboBox.Size = new System.Drawing.Size(200, 20);
            this.tradeOptionComboBox.TabIndex = 19;
            this.tradeOptionComboBox.Text = "필수 정정 구분 선택";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.Controls.Add(this.searchButton);
            this.flowLayoutPanel1.Controls.Add(this.searchTextBox);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(299, 60);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(200, 21);
            this.flowLayoutPanel1.TabIndex = 22;
            // 
            // searchButton
            // 
            this.searchButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.searchButton.Location = new System.Drawing.Point(3, 3);
            this.searchButton.Name = "searchButton";
            this.searchButton.Size = new System.Drawing.Size(75, 21);
            this.searchButton.TabIndex = 0;
            this.searchButton.Text = "검색입력";
            this.searchButton.UseVisualStyleBackColor = true;
            // 
            // searchTextBox
            // 
            this.searchTextBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.searchTextBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.searchTextBox.Location = new System.Drawing.Point(84, 3);
            this.searchTextBox.Name = "searchTextBox";
            this.searchTextBox.Size = new System.Drawing.Size(100, 21);
            this.searchTextBox.TabIndex = 1;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Controls.Add(this.cancelButton, 1, 1);
            this.tableLayoutPanel3.Controls.Add(this.changeButton, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.sellButton, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.buyButton, 0, 0);
            this.tableLayoutPanel3.Location = new System.Drawing.Point(768, 180);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 2;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(503, 73);
            this.tableLayoutPanel3.TabIndex = 2;
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.cancelButton.Location = new System.Drawing.Point(254, 39);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(246, 31);
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "취소(수동)";
            this.cancelButton.UseVisualStyleBackColor = false;
            // 
            // changeButton
            // 
            this.changeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.changeButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.changeButton.ForeColor = System.Drawing.Color.White;
            this.changeButton.Location = new System.Drawing.Point(3, 39);
            this.changeButton.Name = "changeButton";
            this.changeButton.Size = new System.Drawing.Size(245, 31);
            this.changeButton.TabIndex = 2;
            this.changeButton.Text = "정정(수동)";
            this.changeButton.UseVisualStyleBackColor = false;
            // 
            // sellButton
            // 
            this.sellButton.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sellButton.BackColor = System.Drawing.SystemColors.Highlight;
            this.sellButton.ForeColor = System.Drawing.Color.White;
            this.sellButton.Location = new System.Drawing.Point(254, 3);
            this.sellButton.Name = "sellButton";
            this.sellButton.Size = new System.Drawing.Size(246, 30);
            this.sellButton.TabIndex = 1;
            this.sellButton.Text = "매도주문(수동)";
            this.sellButton.UseVisualStyleBackColor = false;
            // 
            // buyButton
            // 
            this.buyButton.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buyButton.BackColor = System.Drawing.Color.Red;
            this.buyButton.ForeColor = System.Drawing.Color.White;
            this.buyButton.Location = new System.Drawing.Point(3, 3);
            this.buyButton.Name = "buyButton";
            this.buyButton.Size = new System.Drawing.Size(245, 30);
            this.buyButton.TabIndex = 0;
            this.buyButton.Text = "매수주문(수동)";
            this.buyButton.UseVisualStyleBackColor = false;
            // 
            // conditionDataGridView
            // 
            this.conditionDataGridView.AllowUserToAddRows = false;
            this.conditionDataGridView.AllowUserToDeleteRows = false;
            this.conditionDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.conditionDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.conditionDataGridView.BackgroundColor = System.Drawing.SystemColors.ActiveCaption;
            this.conditionDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.conditionDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.조건식_조건번호,
            this.조건식_조건식명});
            this.conditionDataGridView.Location = new System.Drawing.Point(5, 211);
            this.conditionDataGridView.MultiSelect = false;
            this.conditionDataGridView.Name = "conditionDataGridView";
            this.conditionDataGridView.RowHeadersVisible = false;
            this.conditionDataGridView.RowTemplate.Height = 23;
            this.conditionDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.conditionDataGridView.Size = new System.Drawing.Size(197, 476);
            this.conditionDataGridView.TabIndex = 3;
            // 
            // 조건식_조건번호
            // 
            this.조건식_조건번호.HeaderText = "조건번호";
            this.조건식_조건번호.Name = "조건식_조건번호";
            this.조건식_조건번호.ReadOnly = true;
            // 
            // 조건식_조건식명
            // 
            this.조건식_조건식명.HeaderText = "조건명";
            this.조건식_조건식명.Name = "조건식_조건식명";
            this.조건식_조건식명.ReadOnly = true;
            // 
            // condigionItemDataGridView
            // 
            this.condigionItemDataGridView.AllowUserToAddRows = false;
            this.condigionItemDataGridView.AllowUserToDeleteRows = false;
            this.condigionItemDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.condigionItemDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.condigionItemDataGridView.BackgroundColor = System.Drawing.SystemColors.ActiveCaption;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.condigionItemDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.condigionItemDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.condigionItemDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.조건종목_종목코드,
            this.조건종목_종목명,
            this.조건종목_현재가,
            this.조건종목_전일대비,
            this.조건종목_등락율,
            this.조건종목_거래량});
            this.condigionItemDataGridView.Location = new System.Drawing.Point(208, 211);
            this.condigionItemDataGridView.MultiSelect = false;
            this.condigionItemDataGridView.Name = "condigionItemDataGridView";
            this.condigionItemDataGridView.RowHeadersVisible = false;
            this.condigionItemDataGridView.RowTemplate.Height = 23;
            this.condigionItemDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.condigionItemDataGridView.Size = new System.Drawing.Size(553, 476);
            this.condigionItemDataGridView.TabIndex = 4;
            // 
            // 조건종목_종목코드
            // 
            this.조건종목_종목코드.HeaderText = "종목코드";
            this.조건종목_종목코드.Name = "조건종목_종목코드";
            this.조건종목_종목코드.ReadOnly = true;
            // 
            // 조건종목_종목명
            // 
            this.조건종목_종목명.HeaderText = "종목명";
            this.조건종목_종목명.Name = "조건종목_종목명";
            this.조건종목_종목명.ReadOnly = true;
            // 
            // 조건종목_현재가
            // 
            this.조건종목_현재가.HeaderText = "현재가";
            this.조건종목_현재가.Name = "조건종목_현재가";
            this.조건종목_현재가.ReadOnly = true;
            // 
            // 조건종목_전일대비
            // 
            this.조건종목_전일대비.HeaderText = "전일대비";
            this.조건종목_전일대비.Name = "조건종목_전일대비";
            this.조건종목_전일대비.ReadOnly = true;
            // 
            // 조건종목_등락율
            // 
            this.조건종목_등락율.HeaderText = "등락율";
            this.조건종목_등락율.Name = "조건종목_등락율";
            this.조건종목_등락율.ReadOnly = true;
            // 
            // 조건종목_거래량
            // 
            this.조건종목_거래량.HeaderText = "거래량";
            this.조건종목_거래량.Name = "조건종목_거래량";
            this.조건종목_거래량.ReadOnly = true;
            // 
            // insertListBox
            // 
            this.insertListBox.FormattingEnabled = true;
            this.insertListBox.ItemHeight = 12;
            this.insertListBox.Location = new System.Drawing.Point(5, 693);
            this.insertListBox.Name = "insertListBox";
            this.insertListBox.Size = new System.Drawing.Size(373, 136);
            this.insertListBox.TabIndex = 5;
            // 
            // deleteListBox
            // 
            this.deleteListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.deleteListBox.FormattingEnabled = true;
            this.deleteListBox.ItemHeight = 12;
            this.deleteListBox.Location = new System.Drawing.Point(385, 693);
            this.deleteListBox.Name = "deleteListBox";
            this.deleteListBox.Size = new System.Drawing.Size(374, 136);
            this.deleteListBox.TabIndex = 6;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.outStandingDataGridView);
            this.groupBox1.Location = new System.Drawing.Point(768, 259);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(503, 216);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "미체결";
            // 
            // outStandingDataGridView
            // 
            this.outStandingDataGridView.AllowUserToAddRows = false;
            this.outStandingDataGridView.AllowUserToDeleteRows = false;
            this.outStandingDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.outStandingDataGridView.BackgroundColor = System.Drawing.SystemColors.ActiveCaption;
            this.outStandingDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.outStandingDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.미체결_주문번호,
            this.미체결_종목코드,
            this.미체결_종목명,
            this.미체결_주문수량,
            this.미체결_미체결수량,
            this.미체결_주문가격,
            this.미체결_현재가,
            this.미체결_체결가,
            this.미체결_주문구분,
            this.미체결_시간});
            this.outStandingDataGridView.Location = new System.Drawing.Point(5, 20);
            this.outStandingDataGridView.MultiSelect = false;
            this.outStandingDataGridView.Name = "outStandingDataGridView";
            this.outStandingDataGridView.RowHeadersVisible = false;
            this.outStandingDataGridView.RowTemplate.Height = 23;
            this.outStandingDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.outStandingDataGridView.Size = new System.Drawing.Size(498, 190);
            this.outStandingDataGridView.TabIndex = 10;
            // 
            // 미체결_주문번호
            // 
            this.미체결_주문번호.HeaderText = "주문번호";
            this.미체결_주문번호.Name = "미체결_주문번호";
            this.미체결_주문번호.ReadOnly = true;
            // 
            // 미체결_종목코드
            // 
            this.미체결_종목코드.HeaderText = "종목코드";
            this.미체결_종목코드.Name = "미체결_종목코드";
            this.미체결_종목코드.ReadOnly = true;
            // 
            // 미체결_종목명
            // 
            this.미체결_종목명.HeaderText = "종목명";
            this.미체결_종목명.Name = "미체결_종목명";
            this.미체결_종목명.ReadOnly = true;
            // 
            // 미체결_주문수량
            // 
            this.미체결_주문수량.HeaderText = "주문수량";
            this.미체결_주문수량.Name = "미체결_주문수량";
            this.미체결_주문수량.ReadOnly = true;
            // 
            // 미체결_미체결수량
            // 
            this.미체결_미체결수량.HeaderText = "미체결수량";
            this.미체결_미체결수량.Name = "미체결_미체결수량";
            this.미체결_미체결수량.ReadOnly = true;
            // 
            // 미체결_주문가격
            // 
            this.미체결_주문가격.HeaderText = "주문가격";
            this.미체결_주문가격.Name = "미체결_주문가격";
            this.미체결_주문가격.ReadOnly = true;
            // 
            // 미체결_현재가
            // 
            this.미체결_현재가.HeaderText = "현재가";
            this.미체결_현재가.Name = "미체결_현재가";
            this.미체결_현재가.ReadOnly = true;
            // 
            // 미체결_체결가
            // 
            this.미체결_체결가.HeaderText = "체결가";
            this.미체결_체결가.Name = "미체결_체결가";
            this.미체결_체결가.ReadOnly = true;
            // 
            // 미체결_주문구분
            // 
            this.미체결_주문구분.HeaderText = "주문구분";
            this.미체결_주문구분.Name = "미체결_주문구분";
            this.미체결_주문구분.ReadOnly = true;
            // 
            // 미체결_시간
            // 
            this.미체결_시간.HeaderText = "시간";
            this.미체결_시간.Name = "미체결_시간";
            this.미체결_시간.ReadOnly = true;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 4;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel4.Controls.Add(this.realizationProfitLabel, 3, 2);
            this.tableLayoutPanel4.Controls.Add(this.label21, 2, 2);
            this.tableLayoutPanel4.Controls.Add(this.당일손익율Label, 1, 2);
            this.tableLayoutPanel4.Controls.Add(this.손익율Label, 0, 2);
            this.tableLayoutPanel4.Controls.Add(this.당일손익금액Label, 3, 1);
            this.tableLayoutPanel4.Controls.Add(this.액Label, 2, 1);
            this.tableLayoutPanel4.Controls.Add(this.예탁자산평가액Label, 1, 1);
            this.tableLayoutPanel4.Controls.Add(this.산평가액Label1, 0, 1);
            this.tableLayoutPanel4.Controls.Add(this.총매입금액Label1, 3, 0);
            this.tableLayoutPanel4.Controls.Add(this.label10, 2, 0);
            this.tableLayoutPanel4.Controls.Add(this.예수금Label, 1, 0);
            this.tableLayoutPanel4.Controls.Add(this.label4, 0, 0);
            this.tableLayoutPanel4.Location = new System.Drawing.Point(5, 20);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 3;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 34F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(500, 100);
            this.tableLayoutPanel4.TabIndex = 8;
            // 
            // realizationProfitLabel
            // 
            this.realizationProfitLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.realizationProfitLabel.AutoSize = true;
            this.realizationProfitLabel.Location = new System.Drawing.Point(378, 77);
            this.realizationProfitLabel.Name = "realizationProfitLabel";
            this.realizationProfitLabel.Size = new System.Drawing.Size(119, 12);
            this.realizationProfitLabel.TabIndex = 33;
            this.realizationProfitLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label21
            // 
            this.label21.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(253, 77);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(119, 12);
            this.label21.TabIndex = 32;
            this.label21.Text = "실현손익";
            this.label21.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // 당일손익율Label
            // 
            this.당일손익율Label.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.당일손익율Label.AutoSize = true;
            this.당일손익율Label.Location = new System.Drawing.Point(128, 77);
            this.당일손익율Label.Name = "당일손익율Label";
            this.당일손익율Label.Size = new System.Drawing.Size(119, 12);
            this.당일손익율Label.TabIndex = 31;
            this.당일손익율Label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // 손익율Label
            // 
            this.손익율Label.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.손익율Label.AutoSize = true;
            this.손익율Label.Location = new System.Drawing.Point(3, 77);
            this.손익율Label.Name = "손익율Label";
            this.손익율Label.Size = new System.Drawing.Size(119, 12);
            this.손익율Label.TabIndex = 30;
            this.손익율Label.Text = "당일손익율";
            this.손익율Label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // 당일손익금액Label
            // 
            this.당일손익금액Label.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.당일손익금액Label.AutoSize = true;
            this.당일손익금액Label.Location = new System.Drawing.Point(378, 43);
            this.당일손익금액Label.Name = "당일손익금액Label";
            this.당일손익금액Label.Size = new System.Drawing.Size(119, 12);
            this.당일손익금액Label.TabIndex = 29;
            this.당일손익금액Label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // 액Label
            // 
            this.액Label.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.액Label.AutoSize = true;
            this.액Label.Location = new System.Drawing.Point(253, 43);
            this.액Label.Name = "액Label";
            this.액Label.Size = new System.Drawing.Size(119, 12);
            this.액Label.TabIndex = 28;
            this.액Label.Text = "당일손익금액";
            this.액Label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // 예탁자산평가액Label
            // 
            this.예탁자산평가액Label.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.예탁자산평가액Label.AutoSize = true;
            this.예탁자산평가액Label.Location = new System.Drawing.Point(128, 43);
            this.예탁자산평가액Label.Name = "예탁자산평가액Label";
            this.예탁자산평가액Label.Size = new System.Drawing.Size(119, 12);
            this.예탁자산평가액Label.TabIndex = 27;
            this.예탁자산평가액Label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // 산평가액Label1
            // 
            this.산평가액Label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.산평가액Label1.AutoSize = true;
            this.산평가액Label1.Location = new System.Drawing.Point(3, 43);
            this.산평가액Label1.Name = "산평가액Label1";
            this.산평가액Label1.Size = new System.Drawing.Size(119, 12);
            this.산평가액Label1.TabIndex = 26;
            this.산평가액Label1.Text = "예탁자산평가액";
            this.산평가액Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // 총매입금액Label1
            // 
            this.총매입금액Label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.총매입금액Label1.AutoSize = true;
            this.총매입금액Label1.Location = new System.Drawing.Point(378, 10);
            this.총매입금액Label1.Name = "총매입금액Label1";
            this.총매입금액Label1.Size = new System.Drawing.Size(119, 12);
            this.총매입금액Label1.TabIndex = 25;
            this.총매입금액Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label10
            // 
            this.label10.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(253, 10);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(119, 12);
            this.label10.TabIndex = 24;
            this.label10.Text = "총매입금액";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // 예수금Label
            // 
            this.예수금Label.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.예수금Label.AutoSize = true;
            this.예수금Label.Location = new System.Drawing.Point(128, 10);
            this.예수금Label.Name = "예수금Label";
            this.예수금Label.Size = new System.Drawing.Size(119, 12);
            this.예수금Label.TabIndex = 23;
            this.예수금Label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.MistyRose;
            this.label4.Location = new System.Drawing.Point(3, 10);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(119, 12);
            this.label4.TabIndex = 22;
            this.label4.Text = "예수금";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.balanceDataGridView);
            this.groupBox2.Controls.Add(this.tableLayoutPanel4);
            this.groupBox2.Location = new System.Drawing.Point(773, 481);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(512, 348);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "잔고";
            // 
            // balanceDataGridView
            // 
            this.balanceDataGridView.AllowUserToAddRows = false;
            this.balanceDataGridView.AllowUserToDeleteRows = false;
            this.balanceDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.balanceDataGridView.BackgroundColor = System.Drawing.SystemColors.ActiveCaption;
            this.balanceDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.balanceDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.잔고_종목코드,
            this.잔고_현재가,
            this.잔고_평가금액,
            this.잔고_종목명,
            this.잔고_보유수량,
            this.잔고_평균단가,
            this.잔고_손익금액,
            this.잔고_손익율,
            this.잔고_매입금액});
            this.balanceDataGridView.Location = new System.Drawing.Point(5, 127);
            this.balanceDataGridView.MultiSelect = false;
            this.balanceDataGridView.Name = "balanceDataGridView";
            this.balanceDataGridView.RowHeadersVisible = false;
            this.balanceDataGridView.RowTemplate.Height = 23;
            this.balanceDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.balanceDataGridView.Size = new System.Drawing.Size(501, 215);
            this.balanceDataGridView.TabIndex = 11;
            // 
            // 잔고_종목코드
            // 
            this.잔고_종목코드.HeaderText = "종목코드";
            this.잔고_종목코드.Name = "잔고_종목코드";
            this.잔고_종목코드.ReadOnly = true;
            // 
            // 잔고_현재가
            // 
            this.잔고_현재가.HeaderText = "현재가";
            this.잔고_현재가.Name = "잔고_현재가";
            // 
            // 잔고_평가금액
            // 
            this.잔고_평가금액.HeaderText = "평가금액";
            this.잔고_평가금액.Name = "잔고_평가금액";
            // 
            // 잔고_종목명
            // 
            this.잔고_종목명.HeaderText = "종목명";
            this.잔고_종목명.Name = "잔고_종목명";
            this.잔고_종목명.ReadOnly = true;
            // 
            // 잔고_보유수량
            // 
            this.잔고_보유수량.HeaderText = "보유수량";
            this.잔고_보유수량.Name = "잔고_보유수량";
            this.잔고_보유수량.ReadOnly = true;
            // 
            // 잔고_평균단가
            // 
            this.잔고_평균단가.HeaderText = "평균단가";
            this.잔고_평균단가.Name = "잔고_평균단가";
            this.잔고_평균단가.ReadOnly = true;
            // 
            // 잔고_손익금액
            // 
            this.잔고_손익금액.HeaderText = "손익금액";
            this.잔고_손익금액.Name = "잔고_손익금액";
            this.잔고_손익금액.ReadOnly = true;
            // 
            // 잔고_손익율
            // 
            this.잔고_손익율.HeaderText = "손익율";
            this.잔고_손익율.Name = "잔고_손익율";
            this.잔고_손익율.ReadOnly = true;
            // 
            // 잔고_매입금액
            // 
            this.잔고_매입금액.HeaderText = "매입금액";
            this.잔고_매입금액.Name = "잔고_매입금액";
            this.잔고_매입금액.ReadOnly = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1285, 841);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.deleteListBox);
            this.Controls.Add(this.insertListBox);
            this.Controls.Add(this.condigionItemDataGridView);
            this.Controls.Add(this.conditionDataGridView);
            this.Controls.Add(this.tableLayoutPanel3);
            this.Controls.Add(this.tableLayoutPanel2);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "Form1";
            this.Text = "Auto Stock";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.takeProfitNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.stopLossNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.totalAmountNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axKHOpenAPI1)).EndInit();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.priceManualNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.amountManulNumericUpDown)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.conditionDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.condigionItemDataGridView)).EndInit();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.outStandingDataGridView)).EndInit();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.balanceDataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.CheckBox buyConditionCheckBox;
        private System.Windows.Forms.ComboBox buyConditionComboBox;
        private System.Windows.Forms.ComboBox sellConditionComboBox;
        private System.Windows.Forms.NumericUpDown takeProfitNumericUpDown;
        private System.Windows.Forms.NumericUpDown stopLossNumericUpDown;
        private System.Windows.Forms.NumericUpDown totalAmountNumericUpDown;
        private System.Windows.Forms.NumericUpDown numericUpDown4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button tradingStartButton;
        private System.Windows.Forms.Button tradingStopButton;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label itemNameLabel;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label userNameLabel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.CheckBox takeProfitCheckBox;
        private System.Windows.Forms.CheckBox stopLossChheckBox;
        private System.Windows.Forms.NumericUpDown priceManualNumericUpDown;
        private System.Windows.Forms.ComboBox accountComboBox;
        private System.Windows.Forms.ComboBox guBunComboBox;
        private System.Windows.Forms.NumericUpDown amountManulNumericUpDown;
        private System.Windows.Forms.ComboBox tradeOptionComboBox;
        private System.Windows.Forms.TextBox itemCodeTextBox;
        private System.Windows.Forms.TextBox originOrderNumtextBox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button changeButton;
        private System.Windows.Forms.Button sellButton;
        private System.Windows.Forms.Button buyButton;
        private System.Windows.Forms.DataGridView conditionDataGridView;
        private System.Windows.Forms.DataGridView condigionItemDataGridView;
        private System.Windows.Forms.ListBox insertListBox;
        private System.Windows.Forms.ListBox deleteListBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView outStandingDataGridView;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.Label realizationProfitLabel;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label 당일손익율Label;
        private System.Windows.Forms.Label 손익율Label;
        private System.Windows.Forms.Label 당일손익금액Label;
        private System.Windows.Forms.Label 액Label;
        private System.Windows.Forms.Label 예탁자산평가액Label;
        private System.Windows.Forms.Label 산평가액Label1;
        private System.Windows.Forms.Label 총매입금액Label1;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label 예수금Label;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DataGridView balanceDataGridView;
        private AxKHOpenAPILib.AxKHOpenAPI axKHOpenAPI1;
        private System.Windows.Forms.DataGridViewTextBoxColumn 잔고_종목코드;
        private System.Windows.Forms.DataGridViewTextBoxColumn 잔고_현재가;
        private System.Windows.Forms.DataGridViewTextBoxColumn 잔고_평가금액;
        private System.Windows.Forms.DataGridViewTextBoxColumn 잔고_종목명;
        private System.Windows.Forms.DataGridViewTextBoxColumn 잔고_보유수량;
        private System.Windows.Forms.DataGridViewTextBoxColumn 잔고_평균단가;
        private System.Windows.Forms.DataGridViewTextBoxColumn 잔고_손익금액;
        private System.Windows.Forms.DataGridViewTextBoxColumn 잔고_손익율;
        private System.Windows.Forms.DataGridViewTextBoxColumn 잔고_매입금액;
        private System.Windows.Forms.DataGridViewTextBoxColumn 미체결_주문번호;
        private System.Windows.Forms.DataGridViewTextBoxColumn 미체결_종목코드;
        private System.Windows.Forms.DataGridViewTextBoxColumn 미체결_종목명;
        private System.Windows.Forms.DataGridViewTextBoxColumn 미체결_주문수량;
        private System.Windows.Forms.DataGridViewTextBoxColumn 미체결_미체결수량;
        private System.Windows.Forms.DataGridViewTextBoxColumn 미체결_주문가격;
        private System.Windows.Forms.DataGridViewTextBoxColumn 미체결_현재가;
        private System.Windows.Forms.DataGridViewTextBoxColumn 미체결_체결가;
        private System.Windows.Forms.DataGridViewTextBoxColumn 미체결_주문구분;
        private System.Windows.Forms.DataGridViewTextBoxColumn 미체결_시간;
        private System.Windows.Forms.DataGridViewTextBoxColumn 조건종목_종목코드;
        private System.Windows.Forms.DataGridViewTextBoxColumn 조건종목_종목명;
        private System.Windows.Forms.DataGridViewTextBoxColumn 조건종목_현재가;
        private System.Windows.Forms.DataGridViewTextBoxColumn 조건종목_전일대비;
        private System.Windows.Forms.DataGridViewTextBoxColumn 조건종목_등락율;
        private System.Windows.Forms.DataGridViewTextBoxColumn 조건종목_거래량;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button searchButton;
        private System.Windows.Forms.TextBox searchTextBox;
        private System.Windows.Forms.DataGridViewTextBoxColumn 조건식_조건번호;
        private System.Windows.Forms.DataGridViewTextBoxColumn 조건식_조건식명;
    }
}

