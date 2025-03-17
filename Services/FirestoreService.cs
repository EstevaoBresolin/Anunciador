using AnunciadorV1.Models;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace AnunciadorV1.Services
{
    public class FirestoreService
    {
        private readonly IJSRuntime _jsRuntime;

        public FirestoreService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
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
                Endereco = dados.TryGetValue("endereco", out var localizacao) ? localizacao.ToString() : "Localização não informada",
                Numero = dados.TryGetValue("numero", out var numero) && int.TryParse(numero?.ToString(), out var numeroConvertido) ? numeroConvertido : 0,
                Categoria = dados.TryGetValue("categoria", out var categoria) && int.TryParse(categoria?.ToString(), out var categoriaConvertida) ? categoriaConvertida : 0,
                Instagram = dados.TryGetValue("instagram", out var instagram) ? instagram.ToString() : "instagram não disponível"
            };

            return anunciante;
        }
    }
}
