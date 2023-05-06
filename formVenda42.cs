using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SistemaLojaCosmeticosInfo42
{
    public partial class formVenda42 : Form
    {
        public formVenda42()
        {
            InitializeComponent();
        }

        //CRIAR A LISTA QUE SERA USADA NA GRID DE PRODUTOS VENDIDOS
        private List<classItemsVenda> ListaItemsVenda = new List<classItemsVenda>();

        //VARIAVEL PARA CALCULAR VALOR TOTAL DA VENDA
        private decimal VendaTotal = 0;

        //VARIAVEL PARA CONTAR QUANTAS LINHAS FORAM ADICIONADAS NA GRID DE VENDA
        private int ItemsVenda = 0;
        
        private void formVenda42_Load(object sender, EventArgs e)
        {
            //CARREGAR DATA DA VENDA
            txtDataVenda.Text = DateTime.Now.ToShortDateString();

            //COMBO PARCELAMENTO
            cbParcelamento.Items.Add("2x sem juros");
            cbParcelamento.Items.Add("3x sem juros");
            cbParcelamento.Items.Add("4x sem juros");
            cbParcelamento.Items.Add("5x sem juros");
            cbParcelamento.Items.Add("6x sem juros");
            cbParcelamento.Items.Add("7x sem juros");
            cbParcelamento.Items.Add("8x sem juros");
            cbParcelamento.SelectedIndex = -1;

            //COMBO FORMA DE PAGAMENTO
            cbFormaPagamento.Items.Add("Cartão de Crédito");
            cbFormaPagamento.Items.Add("Cartão de Débito");
            cbFormaPagamento.Items.Add("Dinheiro");
            cbFormaPagamento.Items.Add("Pix");
            cbFormaPagamento.SelectedIndex = -1;

            //CARREGAR COMBO FUNCIONARIO
            classFuncionario cFuncionario = new classFuncionario();
            cbFuncionario.DataSource = cFuncionario.CarregaFuncionarioVenda();
            //EXIBIR NA COMBO
            cbFuncionario.DisplayMember = "nome";
            //GUARDAR NO BD
            cbFuncionario.ValueMember = "codigo_funcionario";
            cbFuncionario.SelectedIndex = -1;
        }

        private void btSair_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Tem certeza que deseja sair?", "Sistema Loja de Cosméticos", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                this.Close();
            }
        }

        private void btBuscaCliente_Click(object sender, EventArgs e)
        {
            //CRIAR OBJETO DA CLASSE FUNCIONÁRIO PARA USAR OS MÉTODOS DE CONSULTA
            classCliente cCliente = new classCliente();

            if (txtPesqCliente.Text != "")
            {
                dgvCliente.DataSource = cCliente.PesquisaClienteVenda(txtPesqCliente.Text);
            }
            else//VALIDAÇÃO DA CAIXINHA NOME
            {
                MessageBox.Show("Favor informar um nome de um cliente!", "Sistema Loja Cosméticos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPesqCliente.Focus();
            }
        }

        private void btBuscaProduto_Click(object sender, EventArgs e)
        {
            classProduto cProduto = new classProduto();

            if (txtPesqProduto.Text != "")
            {
                dgvProduto.DataSource = cProduto.PesquisaProdutoVenda(txtPesqProduto.Text);
            }
            else//VALIDAÇÃO DA CAIXINHA NOME
            {
                MessageBox.Show("Favor informar um nome de um produto!", "Sistema Loja Cosméticos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPesqCliente.Focus();
            }
        }

        private void btAddProd_Click(object sender, EventArgs e)
        {
            classProduto cProduto = new classProduto();

            
        }

        private void txtQtde_TextChanged(object sender, EventArgs e)
        {
            if (txtQtde.Text == "")
            {
                MessageBox.Show("Favor informar uma quantidade", "Sistema Loja de Cosméticos", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtQtde.Text = "01";
                txtQtde.SelectAll();
            }

            int qtdevendida = Convert.ToInt32(txtQtde.Text);
            int qtdeestoque = Convert.ToInt32(txtQtdeEstoque.Text);

            //VERIFICAR SE A QUANTIDADE É MAIOR QUE A QUANTIDADE EM ESTOQUE
            if (qtdevendida > qtdeestoque)
            {
                MessageBox.Show("A quantidade disponível no estoque é de " + qtdeestoque + " unidades! ", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtQtde.Text = "01";
                txtQtde.SelectAll();
            }
            else
            {
                qtdevendida = Convert.ToInt32(txtQtde.Text);
                decimal valor = Convert.ToDecimal(txtValor.Text);
                txtTotal.Text = (qtdevendida * valor).ToString();
            }
        }

        private void txtValorDesconto_TextChanged(object sender, EventArgs e)
        {
            //SE O USUÁRIO APAGAR O PERCENTUAL DO DESCONTO
            if (txtValorDesconto.Text == "")
            {
                //PEGAR O VALOR DA VENDA
                decimal valortotal = Convert.ToDecimal(txtValorTotal.Text);
                //EXIBIR NA CAIXA
                txtTotalVenda.Text = valortotal.ToString("n2");
                //ZERAR O DESCONTO DO PERCENTUAL
                txtValorDesconto.Text = "0";
                txtValorDesconto.SelectAll();
                //ZERAR O DESCONTO DO CAMPO CALCULADO
                txtTotalDesconto.Text = "0,00";
            }
            else
            {
                //VALOR DA PORCENTAGEM DE DESCONTO
                decimal pDesc = Convert.ToDecimal(txtValorDesconto.Text) / 100;
                //VALOR DA VENDA
                decimal valor = Convert.ToDecimal(txtValorTotal.Text);
                //CALCULO DO DESCONTO
                decimal vfinal = valor - (pDesc * valor);
                //CALCULO DO VALOR FINAL DA VENDA COM DESCONTO
                decimal vVenda = valor - vfinal;
                //EXIBIR O VALOR FINAL DA VENDA COM DESCONTO
                txtTotalDesconto.Text = vVenda.ToString("n2");
                //EXIBIR VALOR DO DESCONTO CALCULADO
                txtTotalVenda.Text = vfinal.ToString("n2");
            }
        }

        private void dgvProduto_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            classProduto cProduto = new classProduto();

            bool resp = cProduto.ConsultaProduto(Convert.ToInt32(dgvProduto.SelectedRows[0].Cells[0].Value));

            if (resp == true)
            {
                txtProduto.Text = cProduto.nome;
                txtQtdeEstoque.Text = cProduto.qtde_estoque.ToString();
                txtValor.Text = cProduto.preco_venda.ToString("n2");
                txtQtde.Text = "01";
                txtQtde_TextChanged(this, new EventArgs());
                txtQtde.Select();
            }
        }

        private void AtualizaGrid()
        {
            //INSTANCIAR CLASSE PRODUTO PARA PEGAR MÉTODO QUE TRAZ O NOME DO PRODUTO
            classProduto cProduto = new classProduto();

            //CRIAR UMA TABELA TEMPORÁRIA
            DataTable dt = new DataTable();

            //CRIAR AS COLUNAS DA GRID
            dt.Columns.Add(new DataColumn("Código"));
            dt.Columns.Add(new DataColumn("Produto"));
            dt.Columns.Add(new DataColumn("Quantidade"));
            dt.Columns.Add(new DataColumn("Valor"));

            //ADICIONAR AS LINHAS DA GRID
            foreach (classItemsVenda item in ListaItemsVenda)
            {
                dt.Rows.Add(item.codigo_produto, cProduto.BuscaNomeProd(item.codigo_produto), item.quantidade, item.valor_total);
                dt.AcceptChanges();
            }
            dgvItens.DataSource = dt;
        }

        private void btAdicionarProduto_Click(object sender, EventArgs e)
        {
            classItemsVenda cItemsVenda = new classItemsVenda();

            //SOMAR VALOR DO ITEM AO TOTAL DA VENDA
            decimal ValorItem = 0;

            //VERIFICAR SE ALGUM PRODUTO FOI SELECIONADO
            if (txtTotal.Text == "")
            {
                MessageBox.Show("Não há produto para ser insetido", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                //SOMAR VALOR DO ITEM SELECINADO AO TOTAL DA VENDA
                ValorItem = Convert.ToDecimal(txtTotal.Text);
                VendaTotal = VendaTotal + ValorItem;
                txtValorTotal.Text = VendaTotal.ToString("n2");
                txtTotalVenda.Text = VendaTotal.ToString("n2");

                //CONTAR ITENS DA VENDA (QTDE DE LINHAS DA GRID)
                ItemsVenda++;

                //INFORMAÇÕES DOS PRODUTOS VENDIDOS
                cItemsVenda.codigo_produto = Convert.ToInt32(dgvProduto.SelectedRows[0].Cells[0].Value);
                cItemsVenda.quantidade = Convert.ToInt32(txtQtde.Text);
                cItemsVenda.valor_total = Convert.ToDecimal(txtTotal.Text);

                //ADICIONAR NA LISTA
                ListaItemsVenda.Add(cItemsVenda);

                //PEGAR DA LISTA A QUANTIDADE DE ITENS
                txtQtdeItens.Text = ItemsVenda.ToString();
                txtQtdeItens.Text = ListaItemsVenda.Count.ToString();

                //CHAMAR MÉTODO QUE CRIA A GRID DE PRODUTOS VENDIDOS
                AtualizaGrid();

                //LIMPAR OS CAMPOS DE PRODUTOS APÓRS ADICIONAR
                txtProduto.Clear();
                txtQtde.Text = "01";
                txtValor.Clear();
                txtTotal.Clear();
                txtQtdeEstoque.Clear();
                txtValorDesconto.Text = "0";
                txtValorDesconto.Select();
            }
        }

        private void Limpar()
        {
            cbFuncionario.SelectedIndex = -1;
            txtPesqCliente.Clear();
            dgvCliente.DataSource = null;
            txtPesqProduto.Clear();
            dgvProduto.DataSource = null;

            ListaItemsVenda.Clear();
            AtualizaGrid();
            dgvItens.Refresh();

            txtValorTotal.Text = "0";
            txtTotalVenda.Text = "0";
            txtValorDesconto.Text = "0";
            txtTotalDesconto.Text = "0";

            txtQtdeItens.Clear();
            cbFormaPagamento.SelectedIndex = -1;
            VendaTotal = 0;
        }

        private void CalculaEstoque(int qtde, int cod)
        {
            classProduto cProduto = new classProduto();
            cProduto.ConsultaProduto(cod);
            int estoque = cProduto.qtde_estoque;
            cProduto.AtualizaEstoque(estoque - qtde, cod);
        }

        private void CamposObrigatorios()
        {
            gbFuncionario.BackColor = Color.LemonChiffon;
            gbCliente.BackColor = Color.LemonChiffon;
            gbItemsVenda.BackColor = Color.LemonChiffon;
            cbFormaPagamento.BackColor = Color.LemonChiffon;

        }

        private void btFechaVenda_Click(object sender, EventArgs e)
        {
            //VERIFICAR CAMPOS OBRIGATÓRIOS
            if (cbFuncionario.SelectedIndex != -1 && dgvCliente.DataSource != null && dgvItens.DataSource != null && cbFormaPagamento.SelectedIndex != -1)
            {
                classVenda cVenda = new classVenda();

                //MANDAR INFORMAÇÕES DA TABELA VENDA
                cVenda.codigo_cliente = Convert.ToInt32(dgvCliente.SelectedRows[0].Cells[0].Value);
                cVenda.codigo_funcionario = Convert.ToInt32(cbFuncionario.SelectedValue);
                cVenda.forma_pagamento = cbFormaPagamento.SelectedItem.ToString();
                cVenda.valor_total = Convert.ToDecimal(txtTotalVenda.Text);
                cVenda.desconto = Convert.ToDecimal(txtTotalDesconto.Text);

                //CHAMAR MÉTODO CADASTRAR VENDA
                bool resp = cVenda.CadastraVenda();

                if (resp == true) //SE O INSERT DA VENDA DEU CERTO
                {
                    //LAÇO DE REPETIÇÃO COM OS PRODUTOS VENDIDOS E BAIXA ESTOQUE
                    foreach (classItemsVenda item in ListaItemsVenda)
                    {
                        //MANDAR PARA A TABELA DE ITENS O CÓDIGO DA VENDA
                        item.codigo_venda = cVenda.codigo_venda;
                        //EXECUTA O INSERT DA TABELA DE ITENS
                        resp = item.CadastraItemVenda();

                        //BAIXA ESTOQUE
                        CalculaEstoque(item.quantidade, item.codigo_produto);
                    }
                    if (resp == true)
                    {
                        MessageBox.Show("Venda realizada com Sucesso", "Sistema Loja de Cosmétios", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Limpar();
                    }
                }
                else
                {
                    MessageBox.Show("Erro ao realizar Venda", "Sistema Loja de Cosméticos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Favor preencher todos os campos", "Sistema Loja de Cosméticos", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                CamposObrigatorios();
            }
        }

        private void txtQtdeEstoque_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != 08 && e.KeyChar != 13 && e.KeyChar != 32 && e.KeyChar != 44)
            {
                e.Handled = true;
                MessageBox.Show("Este campo aceita apenas Números!", "Sistema Loja de Cosméticos",
                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void txtQtde_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != 08 && e.KeyChar != 13 && e.KeyChar != 32 && e.KeyChar != 44)
            {
                e.Handled = true;
                MessageBox.Show("Este campo aceita apenas Números!", "Sistema Loja de Cosméticos",
                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void txtValorDesconto_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != 08 && e.KeyChar != 13 && e.KeyChar != 32 && e.KeyChar != 44)
            {
                e.Handled = true;
                MessageBox.Show("Este campo aceita apenas Números!", "Sistema Loja de Cosméticos",
                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void txtValorPago_TextChanged(object sender, EventArgs e)
        {
            if (txtValorPago.Text != "")
            {
                //PEGAR O VALOR DA VENDA
                decimal totalvenda = Convert.ToDecimal(txtTotalVenda.Text);
                decimal valorpago = Convert.ToDecimal(txtValorPago.Text);
                decimal troco = 0;

                //EXIBIR NA CAIXA
                //txtTroco.Text = Convert.ToString(valorpago - totalvenda);

                troco = valorpago - totalvenda;
                txtTroco.Text = troco.ToString();
            }
        }

        private void cbFormaPagamento_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(cbFormaPagamento.SelectedIndex == 0)//Crédito
            {
                txtValorPago.Enabled = false;
                cbParcelamento.Enabled = true;
            }

            if(cbFormaPagamento.SelectedIndex == 1)//Débito
            {
                txtValorPago.Enabled = false;
                cbParcelamento.Enabled = true;
            }

            if (cbFormaPagamento.SelectedIndex == 2)//Dinheiro
            {
                txtValorPago.Enabled = true;
                cbParcelamento.Enabled = false;
            }

            if (cbFormaPagamento.SelectedIndex == 3)//Pix
            {
                txtValorPago.Enabled = false;
                cbParcelamento.Enabled = false;
            }
        }

        private void cbParcelamento_SelectedIndexChanged(object sender, EventArgs e)
        {
            string parcelas = cbParcelamento.SelectedItem.ToString();
            decimal ValorCredito = 0;
            decimal ValorParcela = 0;

            switch(parcelas)
            {
                case "2x sem juros":
                    if (cbParcelamento.SelectedIndex == 0)
                    {
                        ValorCredito = Convert.ToDecimal(txtTotalVenda.Text);
                        ValorParcela = ValorCredito / 2;
                        txtValorParcela.Text = ValorParcela.ToString("n2");
                    }
                    break;

                case "3x sem juros":
                    if (cbParcelamento.SelectedIndex == 1)
                    {
                        ValorCredito = Convert.ToDecimal(txtTotalVenda.Text);
                        ValorParcela = ValorCredito / 3;
                        txtValorParcela.Text = ValorParcela.ToString("n2");
                    }
                    break;

                case "4x sem juros":
                    if (cbParcelamento.SelectedIndex == 2)
                    {
                        ValorCredito = Convert.ToDecimal(txtTotalVenda.Text);
                        ValorParcela = ValorCredito / 4;
                        txtValorParcela.Text = ValorParcela.ToString("n2");
                    }
                    break;

                case "5x sem juros":
                    if (cbParcelamento.SelectedIndex == 3)
                    {
                        ValorCredito = Convert.ToDecimal(txtTotalVenda.Text);
                        ValorParcela = ValorCredito / 5;
                        txtValorParcela.Text = ValorParcela.ToString("n2");
                    }
                    break;

                case "6x sem juros":
                    if (cbParcelamento.SelectedIndex == 4)
                    {
                        ValorCredito = Convert.ToDecimal(txtTotalVenda.Text);
                        ValorParcela = ValorCredito / 6;
                        txtValorParcela.Text = ValorParcela.ToString("n2");
                    }
                    break;

                case "7x sem juros":
                    if (cbParcelamento.SelectedIndex == 5)
                    {
                        ValorCredito = Convert.ToDecimal(txtTotalVenda.Text);
                        ValorParcela = ValorCredito / 7;
                        txtValorParcela.Text = ValorParcela.ToString("n2");
                    }
                    break;

                case "8x sem juros":
                    if (cbParcelamento.SelectedIndex == 6)
                    {
                        ValorCredito = Convert.ToDecimal(txtTotalVenda.Text);
                        ValorParcela = ValorCredito / 8;
                        txtValorParcela.Text = ValorParcela.ToString("n2");
                    }
                    break;
            }
        }

        private void btRemove_Click(object sender, EventArgs e)
        {
            if(dgvItens.Rows.Count > 0)
            {
                if(MessageBox.Show("Deseja Remover o Produto Selecionado?", "Sistema Loja de Cosméticos", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    decimal valor = Convert.ToDecimal(dgvItens.SelectedRows[0].Cells[3].Value);
                    VendaTotal = VendaTotal - valor;
                    txtTotalVenda.Text = VendaTotal.ToString();
                    txtValorTotal.Text = VendaTotal.ToString();

                    ListaItemsVenda.RemoveAt(dgvItens.CurrentRow.Index);
                    AtualizaGrid();

                    txtQtdeItens.Text = ListaItemsVenda.Count.ToString();
                    txtValorDesconto_TextChanged(this, new EventArgs());
                }
            }
            else
            {
                MessageBox.Show("Não há produtos para ser removido", "Sistema Loja de Cosméticos", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
