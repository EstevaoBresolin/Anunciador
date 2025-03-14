using Microsoft.JSInterop;
using System.Collections.Generic;
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

        public async Task<List<Dictionary<string, object>>> GetAnunciantes()
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
            return await _jsRuntime.InvokeAsync<List<Dictionary<string, object>>>("firebaseService.getAnunciantes", db);
        }
    }
}
