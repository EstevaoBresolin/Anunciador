using System.Text.Json;
using AnunciadorV1.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text.Json;

namespace AnunciadorV1.Services
{
    public class FirestoreService
    {
        private readonly IJSRuntime _jsRuntime;
        private DotNetObjectReference<FirestoreService>? _dotNetRef;
        private bool _isAuthenticated = false;
        public string NomeUsuario = "";
        public string? UidUsuario { get; private set; } = null;
        public bool TemAssinaturaAtiva { get; private set; } = false;

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
            _isAuthenticated = isAuthenticated;
            OnAuthStateChanged?.Invoke();

            if (isAuthenticated && !string.IsNullOrEmpty(UidUsuario))
            {
                await IniciarMonitoramentoAssinaturaAsync(UidUsuario);
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

        public async Task<List<Anunciante>> GetAnunciantes()
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

            // Passar a instância do Firestore (db) para a função getAnunciantes
             var result = await _jsRuntime.InvokeAsync<List<Dictionary<string, object>>>("firebaseService.getAnunciantes", db);

            return result.Select(d => ConverterParaAnunciante(d)).ToList();
        }

        private Anunciante ConverterParaAnunciante(Dictionary<string, object> dados)
        {
            var anunciante =  new Anunciante
            {
                Nome = dados.TryGetValue("nome", out var nome) ? nome.ToString() : "Nome não informado",
                Descricao = dados.TryGetValue("descricao", out var descricao) ? descricao.ToString() : "",
                Titulo = dados.TryGetValue("titulo", out var titulo) ? titulo.ToString() : "Titulo não informado",
                EnderecoFoto = dados.TryGetValue("enderecoFoto", out var enderecoFoto) ? enderecoFoto.ToString() : "Endereço da foto não informado",
                Endereco = dados.TryGetValue("endereco", out var endereco) ? endereco.ToString() : "Localização não informada",
                Numero = dados.TryGetValue("numero", out var numero) && int.TryParse(numero?.ToString(), out var numeroConvertido) ? numeroConvertido : 0,
                Categoria = dados.TryGetValue("categoria", out var categoria) && int.TryParse(categoria?.ToString(), out var categoriaConvertida) ? categoriaConvertida : 0,
                Instagram = dados.TryGetValue("instagram", out var instagram) ? instagram.ToString() : "instagram não disponível"
            };

            return anunciante;
        }

        public async Task<bool> Login(string email, string senha)
        {
            var user = await _jsRuntime.InvokeAsync<object>("firebaseService.login", email, senha);
            if(user != null)
            {
                NomeUsuario = email.Split('@')[0];

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
        public void SetUserLogged(string email)
        {
            _isAuthenticated = true;
            NomeUsuario = email.Split('@')[0];
            OnAuthStateChanged?.Invoke();
        }

        [JSInvokable]
        public void SetUserLoggedOut()
        {
            _isAuthenticated = false;
            NomeUsuario = "";
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
            var temAssinatura = await _jsRuntime.InvokeAsync<bool>("firebaseService.getActiveSubscriptionStatus", userUid);
            TemAssinaturaAtiva = temAssinatura;
        }
        public async Task<string> UploadImagemFromInput(ElementReference inputRef, string path)
        {
            return await _jsRuntime.InvokeAsync<string>("firebaseService.uploadImageFromInput", inputRef, path);
        }
        public async Task<string> GetCurrentUserId()
        {
            return await _jsRuntime.InvokeAsync<string>("firebaseService.getCurrentUserId");
        }
    }
}
