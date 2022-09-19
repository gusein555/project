using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Test.DataModel;

namespace Test
{
    public partial class FormMAIN : Form
    {
        public FormMAIN()
        {
            InitializeComponent();
        }


        //МЕТОД ЗАГРУЗКИ ФОРМЫ
        private void FormMAIN_Load(object sender, EventArgs e)
        {

            using (DBcontext context = new DBcontext())
            {
                foreach (Category category in context.Category)
                {
                    dataGridViewCategoryList.Rows.Add(category.id, category.Name);

                    ComboboxItem item = new ComboboxItem();
                    item.Text = category.Name;
                    item.Value = category.id;

                    comboBoxCategory.Items.Add(item);
                }

                foreach (Product product in context.Product)
                {
                    foreach (Category category in context.Category)
                    {
                        if (product.CategoryId == category.id)
                        {
                            dataGridViewProductList.Rows.Add(product.id, product.Name, product.Price, category.Name);
                        }
                    }
                }
            }
        }


        private void buttonAddCategory_Click(object sender, EventArgs e)
        {
            Category category = new Category();

            category.Name = textBoxCategory.Text;

            var contex = new ValidationContext(category);
            var results = new List<ValidationResult>();
            if (!Validator.TryValidateObject(category, contex, results, true))
            {
                string err = string.Empty;
                foreach (var error in results)
                {
                    err = error.ErrorMessage + "\n";
                }

                MessageBox.Show(err, "Не удалось добавить новую категорию");
            }
            else
            {
                try
                {
                    using (DBcontext context = new DBcontext())
                    {

                        context.Category.Add(category);
                        context.SaveChanges();

                        MessageBox.Show("Категория успешно добавлена");

                        dataGridViewCategoryList.Rows.Clear();
                        foreach (Category categoryUpdate in context.Category)
                        {
                            dataGridViewCategoryList.Rows.Add(categoryUpdate.id, categoryUpdate.Name);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message,"Ошибка");
                }
            }    
            
        }

        private void buttonProduct_Click(object sender, EventArgs e)
        {
            Product product = new Product();

            product.Name = textBoxProduct.Text;
            product.Price = Convert.ToDecimal(textBoxProductPrice.Text.Replace(",","."));

            try
            {
                product.CategoryId = Convert.ToInt32((comboBoxCategory.SelectedItem as ComboboxItem).Value.ToString());
            }
            catch 
            {
                MessageBox.Show("Укажите категорию");
                return;
            }

            var contex = new ValidationContext(product);
            var results = new List<ValidationResult>();
            if (!Validator.TryValidateObject(product, contex, results, true))
            {
                string err = string.Empty;
                foreach (var error in results)
                {
                    err = error.ErrorMessage + "\n";
                }

                MessageBox.Show(err, "Не удалось добавить новый товар");
            }
            else
            {
                try
                {
                    using (DBcontext context = new DBcontext())
                    {
                        context.Product.Add(product);
                        context.SaveChanges();

                        MessageBox.Show("Товар успешно добавлен");

                        dataGridViewProductList.Rows.Clear();
                        foreach (Product productUpdate in context.Product)
                        {
                            foreach (Category category in context.Category)
                            {
                                if (productUpdate.CategoryId == category.id)
                                {
                                    dataGridViewProductList.Rows.Add(productUpdate.id, productUpdate.Name, productUpdate.Price, category.Name);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка");
                }
            }

        }

        private void textBoxProductPrice_TextChanged(object sender, EventArgs e)
        {
            if (textBoxProductPrice.Text.Trim() == string.Empty)
            {
                textBoxProductPrice.Text = "0";
            }

            textBoxProductPrice.Text = textBoxProductPrice.Text.Trim();
        }

        private void textBoxProductPrice_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (textBoxProductPrice.Text.Contains(".") && (e.KeyChar == 'б' || e.KeyChar == 'ю' || e.KeyChar == '.' || e.KeyChar == ','))
            {
                e.Handled = true;
            }
            else if (textBoxProductPrice.Text.Length == 0 && (e.KeyChar == 'б' || e.KeyChar == 'ю' || e.KeyChar == '.' || e.KeyChar == ','))
            {
                e.Handled = true;
            }
            else if (e.KeyChar == 'б')
            {
                e.KeyChar = '.';
            }
            else if (e.KeyChar == 'ю')
            {
                e.KeyChar = '.';
            }
            else if (e.KeyChar == '.')
            {
                e.KeyChar = '.';
            }
            else if (e.KeyChar == ',')
            {
                e.KeyChar = '.';
            }

            if ((e.KeyChar <= 47 || e.KeyChar >= 58) && e.KeyChar != 8 && e.KeyChar != 44 && e.KeyChar != 46)
                e.Handled = true;
        }
    }

    public class ComboboxItem
    {
        public string Text { get; set; }
        public object Value { get; set; }

        public override string ToString()
        {
            return Text;
        }
    }
}
