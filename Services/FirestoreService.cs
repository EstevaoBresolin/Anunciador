using System.Text.Json;
using AnunciadorV1.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text.Json;
using System.ComponentModel.DataAnnotations;

namespace AnunciadorV1.Services
{
    public class FirestoreService
    {
        private readonly IJSRuntime _jsRuntime;
        private DotNetObjectReference<FirestoreService>? _dotNetRef;
        private bool _isAuthenticated = false;
        public string NomeUsuario = "";
        public string Email = "";
        public string? UidUsuario { get; private set; } = null;
        public bool TemAssinaturaAtiva { get; private set; } = false;
        public bool IsAdm { get; private set; } = false;
        public bool IsAuthenticated
        {
            get => _isAuthenticated;
            set
            {
                if (_isAuthenticated != value)
                {
                    _isAuthenticated = value;
                    OnAuthStateChanged?.Invoke();
                }
            }
        }

        public event Action? OnAuthStateChanged;

        public FirestoreService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
            _dotNetRef = DotNetObjectReference.Create(this);
            _jsRuntime.InvokeVoidAsync("firebaseService.setDotnetHelper", _dotNetRef);
        }
        public async void SetAuthenticated(bool isAuthenticated)
        {
            try
            {
                _isAuthenticated = isAuthenticated;
                OnAuthStateChanged?.Invoke();

                if (isAuthenticated && !string.IsNullOrEmpty(UidUsuario))
                {
                    await IniciarMonitoramentoAssinaturaAsync(UidUsuario);
                }
            }
            catch(Exception ex)
            {

            }
            
        }
        public async Task AddAnunciante(object anunciante)
        {
            var firebaseConfig = new
            {
                apiKey = "AIzaSyC4yQ7qg9u8y7h9wh7y0GZygKdMgaoGDE8",
                authDomain = "conectemembros.firebaseapp.com",
                projectId = "conectemembros",
                storageBucket = "conectemembros.firebasestorage.app",
                messagingSenderId = "882219848815",
                appId = "1:882219848815:web:f04520e555863ccf2d699a"
            };
            var db = await _jsRuntime.InvokeAsync<IJSObjectReference>("firebaseService.initializeApp", firebaseConfig);

            var docId = await _jsRuntime.InvokeAsync<string>("firebaseService.addAnunciante", db, anunciante);

        }

        public async Task UpdateAnunciante(object anunciante, string id)
        {
            var docId = await _jsRuntime.InvokeAsync<string>("firebaseService.updateAnuncio", id, anunciante);
        }

        public async Task ExcluirAnuncio(string id)
        {
            var docId = await _jsRuntime.InvokeAsync<string>("firebaseService.deleteAnuncio", id);

        }

        public async Task<List<Anunciante>> GetAnunciantes()
        {
            //var firebaseConfig = new
            //{
            //    apiKey = "AIzaSyC4yQ7qg9u8y7h9wh7y0GZygKdMgaoGDE8",
            //    authDomain = "conectemembros.firebaseapp.com",
            //    projectId = "conectemembros",
            //    storageBucket = "conectemembros.firebasestorage.app",
            //    messagingSenderId = "882219848815",
            //    appId = "1:882219848815:web:f04520e555863ccf2d699a"
            //};

            //var db = await _jsRuntime.InvokeAsync<IJSObjectReference>("firebaseService.initializeApp", firebaseConfig);

            // Passar a instância do Firestore (db) para a função getAnunciantes
             var result = await _jsRuntime.InvokeAsync<List<Dictionary<string, object>>>("firebaseService.getAnunciantes");

            return result.Select(d => ConverterParaAnunciante(d)).ToList();
        }

        public async Task<List<Anunciante>> GetAnunciosUsuario()
        {
            var result = await _jsRuntime.InvokeAsync<List<Dictionary<string, object>>>("firebaseService.getAnunciosUsuario", UidUsuario);

            return result.Select(d => ConverterParaAnunciante(d)).ToList();
        }

        public async Task<List<Anunciante>> GetAnunciosUsuario(string uid)
        {
            var result = await _jsRuntime.InvokeAsync<List<Dictionary<string, object>>>("firebaseService.getAnunciosUsuario", uid);

            return result.Select(d => ConverterParaAnunciante(d)).ToList();
        }

        public async Task<Anunciante> GetAnuncioPorId(string id)
        {
            //var result = await _jsRuntime.InvokeAsync<List<Dictionary<string, object>>>("firebaseService.getAnuncioPorId", id);

            //return result.Select(d => ConverterParaAnunciante(d)).ToList();

            var dados = await _jsRuntime.InvokeAsync<Dictionary<string, object>>("firebaseService.getAnuncioPorId", id);
            if (dados == null) return null;

            return ConverterParaAnunciante(dados);
        }

        private Anunciante ConverterParaAnunciante(Dictionary<string, object> dados)
        {
            //long numeroConvertido = 0;
            //if (dados.TryGetValue("numero", out var numeroRaw) &&
            //    numeroRaw is JsonElement jsonNumero &&
            //    jsonNumero.ValueKind == JsonValueKind.Number)
            //{
            //    try
            //    {
            //        numeroConvertido = jsonNumero.GetInt64();
            //    }
            //    catch
            //    {
            //        numeroConvertido = 0;
            //    }
            //}

            try
            {
                long numeroConvertido = dados.TryGetValue("numero", out var numero) && long.TryParse(numero?.ToString(), out var tempNumero)
                ? tempNumero
                : 0;

                var anunciante = new Anunciante
                {
                    Id = dados.TryGetValue("idFirestore", out var idFirestore) ? idFirestore.ToString() : null,
                    Nome = dados.TryGetValue("nome", out var nome) ? nome.ToString() : "Nome não informado",
                    Descricao = dados.TryGetValue("descricao", out var descricao) ? descricao.ToString() : "",
                    Titulo = dados.TryGetValue("titulo", out var titulo) ? titulo.ToString() : "Titulo não informado",
                    EnderecoFoto = dados.TryGetValue("enderecoFoto", out var enderecoFoto) ? enderecoFoto.ToString() : "Endereço da foto não informado",
                    Endereco = dados.TryGetValue("endereco", out var endereco) ? endereco.ToString() : "Localização não informada",
                    Numero = numeroConvertido,
                    Categoria = dados.TryGetValue("categoria", out var categoria) && int.TryParse(categoria?.ToString(), out var categoriaConvertida) ? categoriaConvertida : 0,
                    Instagram = dados.TryGetValue("instagram", out var instagram) ? instagram.ToString() : "instagram não disponível",
                    UidUsuario = dados.TryGetValue("uidUsuario", out var uidUsuario) ? uidUsuario.ToString() : "",
                    AtivoInativo = dados.TryGetValue("ativoInativo", out var ativoInativo) && bool.TryParse(ativoInativo?.ToString(), out var resultado) ? resultado : false,
                };

                return anunciante;
            }
            catch (Exception e)
            {
                // Faça log ou trate o erro adequadamente
                throw new Exception("Erro ao converter dados do anunciante", e);
            }

            return null;
        }

        public async Task<bool> Login(string email, string senha)
        {
            var user = await _jsRuntime.InvokeAsync<object>("firebaseService.login", email, senha);
            if(user != null)
            {
                NomeUsuario = email.Split('@')[0];
                Email = email;
                UidUsuario = await _jsRuntime.InvokeAsync<string>("firebaseService.getCurrentUserUid");

                _isAuthenticated = true;
                OnAuthStateChanged?.Invoke();
            }
            return user != null;
        }

        public async Task Logout()
        {
            await _jsRuntime.InvokeVoidAsync("firebaseService.logout");
            _isAuthenticated = false;
            SetAuthenticated(false);
            UidUsuario = "";
            TemAssinaturaAtiva = false;
            NomeUsuario = "";
            Email = "";
            OnAuthStateChanged?.Invoke();
        }

        public async Task Registrar(string email, string senha)
        {
            await _jsRuntime.InvokeVoidAsync("firebaseService.registerUser", email, senha);
        }

        public async Task<bool> RecuperarSenha(string email)
        {
            try
            {
                await _jsRuntime.InvokeVoidAsync("firebaseService.recuperarSenha", email);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao enviar e-mail de redefinição: {ex.Message}");
                return false;
            }
        }

        [JSInvokable]
        public void SetUserLogged(string email, string uid)
        {
            _isAuthenticated = true;
            NomeUsuario = email.Split('@')[0];
            Email = email;
            UidUsuario = uid;
            IniciarMonitoramentoAssinaturaAsync(uid);

            OnAuthStateChanged?.Invoke();
        }

        [JSInvokable]
        public void SetUserLoggedOut()
        {
            _isAuthenticated = false;
            NomeUsuario = "";
            Email = "";
            UidUsuario = null;
            OnAuthStateChanged?.Invoke();
        }

        public async Task InicializarAsync()
        {
            _dotNetRef ??= DotNetObjectReference.Create(this);

            // Firebase config aqui dentro
            var firebaseConfig = new
            {
                apiKey = "AIzaSyC4yQ7qg9u8y7h9wh7y0GZygKdMgaoGDE8",
                authDomain = "conectemembros.firebaseapp.com",
                projectId = "conectemembros",
                storageBucket = "conectemembros.firebasestorage.app",
                messagingSenderId = "882219848815",
                appId = "1:882219848815:web:f04520e555863ccf2d699a"
            };

            await _jsRuntime.InvokeVoidAsync("firebaseService.setDotnetHelper", _dotNetRef);
            await _jsRuntime.InvokeVoidAsync("firebaseService.initializeApp", firebaseConfig);
        }

        public async Task<List<Produto>> GetProdutos()
        {
            // Chamar a função JS para buscar os produtos
            var result = await _jsRuntime.InvokeAsync<List<object>>("firebaseService.getProdutos");

            var produtos = new List<Produto>();

            foreach (var item in result)
            {
                var produtoData = JsonSerializer.Deserialize<Dictionary<string, object>>(item.ToString());
                if (produtoData != null)
                {
                    var produto = new Produto
                    {
                        Id = produtoData["id"]?.ToString(),
                        Name = produtoData["name"]?.ToString(),
                        Active = produtoData.ContainsKey("active") && produtoData["active"] is bool active && active,
                        Precos = new List<Preco>()
                    };

                    if (produtoData.ContainsKey("precos"))
                    {
                        var precosElement = produtoData["precos"] as JsonElement?;
                        if (precosElement.HasValue && precosElement.Value.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var precoItem in precosElement.Value.EnumerateArray())
                            {
                                var precoData = JsonSerializer.Deserialize<Dictionary<string, object>>(precoItem.ToString());
                                if (precoData != null)
                                {
                                    var preco = new Preco
                                    {
                                        Id = precoData["id"]?.ToString(),
                                        Currency = precoData["currency"]?.ToString(),
                                        Description = precoData["description"]?.ToString(),
                                        UnitAmount = TryParseDecimal(precoData["unit_amount"])
                                    };

                                    produto.Precos.Add(preco);
                                }
                            }
                        }
                    }


                    produtos.Add(produto);
                }
            }

            return produtos;
        }

        // Função auxiliar para parse seguro de decimal
        private decimal? TryParseDecimal(object value)
        {
            if (value == null)
                return null;

            if (decimal.TryParse(value.ToString(), out var result))
                return result;

            return null;
        }
      
        public async Task IniciarMonitoramentoAssinaturaAsync(string userUid)
        {
            var temAssinatura = await _jsRuntime.InvokeAsync<ResultadoAssinatura>("firebaseService.getActiveSubscriptionStatus", userUid);
            GetAdm();
            TemAssinaturaAtiva = temAssinatura.Sucesso;
        }
       
        //public async Task RedirecionarParaPortal()
        //{
        //    await _jsRuntime.InvokeAsync<string>("firebaseService.getCurrentUserId", NavigationManager.);
        //}

        public async Task<string> UploadImagemFromInput(ElementReference inputRef, string path)
        {
            return await _jsRuntime.InvokeAsync<string>("firebaseService.uploadImageFromInput", inputRef, path);
        }
       
        public async Task<string> GetCurrentUserId()
        {
            return await _jsRuntime.InvokeAsync<string>("firebaseService.getCurrentUserId");
        }

        public async Task<User?> GetStatusAssinatura()
        {
            var result = await _jsRuntime.InvokeAsync<Dictionary<string, object>>("firebaseService.getActiveSubscriptionStatus", UidUsuario);

            if (result == null) return null;

            return new User
            {
                //Ativa = result.TryGetValue("ativa", out var ativa) && Convert.ToBoolean(ativa),
                Status = result.TryGetValue("status", out var status) ? status?.ToString() : "desconhecido",
                Tipo = result.TryGetValue("tipo", out var tipo) ? tipo?.ToString() : "indefinido",
                DataExpiracao = result.TryGetValue("dataExpiracao", out var dataExp) ? dataExp?.ToString() : "indefinida"
            };
        }

        public async Task GetAdm()
        {
            var temAssinatura = await _jsRuntime.InvokeAsync<string>("firebaseService.getAdm");
            if (temAssinatura == UidUsuario)
            {
                IsAdm = true;
            }
            else
            {
                IsAdm = false;
            }
        }

        public async Task<List<Anunciante>> GetAnunciosComAssinaturaExpirada()
        {
            List<Anunciante> anuncios = new();
            var anunciantes = await GetAnunciantes();
            foreach (var anunciante in anunciantes)
            {
                Usuario usuario = new();
                var assinaturaStatus = await GetAssinaturaStatus(anunciante.UidUsuario);
                if (assinaturaStatus == null) continue;
                usuario.AssinaturaAtiva = assinaturaStatus.Sucesso;
                usuario.TemAnuncioAtivo = await VerificarAnuncioAtivo(anunciante.UidUsuario);

                if(usuario.TemAnuncioAtivo == true && usuario.AssinaturaAtiva == false)
                {
                    anuncios.Add(anunciante);
                }
            }
            return anuncios;
        }

        private async Task<ResultadoAssinatura> GetAssinaturaStatus(string userId)
        {
            if (String.IsNullOrEmpty(userId))
            {
                return null;
            }
            var result = await _jsRuntime.InvokeAsync<Dictionary<string, object>>("firebaseService.getActiveSubscriptionStatus", userId);
            return new ResultadoAssinatura
            {
                Sucesso = result != null && result.ContainsKey("status") && result["status"].ToString().Equals("active", StringComparison.OrdinalIgnoreCase)
            };
        }

        private async Task<bool> VerificarAnuncioAtivo(string userId)
        {
            var anuncios = await GetAnunciosUsuario(userId);
            return anuncios.Any(anuncio => anuncio.AtivoInativo);
        }

        public class ResultadoAssinatura
        {
            public bool Sucesso { get; set; }
        }
    }
}
