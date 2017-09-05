﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormApp.AuthWebService;
using Lib.ioc;
using Lib.extension;
using Lib.data;
using Hiwjcn.Core.Domain.Auth;
using Lib.rpc;
using Lib.mvc.auth;

namespace WindowsFormApp
{
    public partial class AuthServiceForm : Form
    {
        public AuthServiceForm()
        {
            InitializeComponent();
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            try
            {
                using (var scope = AppContext.Scope())
                {
                    var repo = scope.Resolve_<IRepository<AuthClient>>();
                    var client = (await repo.GetListAsync(null, 1)).FirstOrDefault();
                    if (client == null)
                    {
                        MessageBox.Show("client is null");
                        return;
                    }
                    var api = new AuthApiFromWcf("http://localhost:59840/Service/AuthApiService.svc");

                    var code = await api.GetAuthCodeByPasswordAsync(client.UID, new List<string>().ToJson(), "32", "53");
                    if (!code.success)
                    {
                        MessageBox.Show(code.msg);
                        return;
                    }
                    var token = await api.GetAccessTokenAsync(client.UID, client.ClientSecretUID, code.data, string.Empty);
                    if (!token.success)
                    {
                        MessageBox.Show(token.msg);
                        return;
                    }
                    MessageBox.Show(token.data.Token);
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }
    }
}