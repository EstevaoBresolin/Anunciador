// Importar os módulos necessários do Firebase SDK
import { initializeApp } from "https://www.gstatic.com/firebasejs/11.4.0/firebase-app.js";
import { getFirestore, collection, getDocs, addDoc } from "https://www.gstatic.com/firebasejs/11.4.0/firebase-firestore.js";

// Inicializar o Firebase com a configuração fornecida
window.firebaseService = {
    initializeApp: function (config) {
        const app = initializeApp(config);  // Inicializa o Firebase com a configuração
        const db = getFirestore(app);  // Obtém a instância do Firestore
        return db;  // Retorna a instância do Firestore
    },

    getAnunciantes: async function (db) {
        if (!db) {
            throw new Error('Firestore não foi inicializado corretamente');
        }

        // Usar a API modular para acessar a coleção
        const anunciantesRef = collection(db, 'anunciantes');  // Referência à coleção
        const querySnapshot = await getDocs(anunciantesRef);  // Obtém os documentos da coleção

        // Mapeia os documentos para um formato legível
        return querySnapshot.docs.map(doc => doc.data());
    },

    addAnunciante: async function (db, data) {
        if (!db) {
            throw new Error('Firestore não foi inicializado corretamente');
        }

        // Referência à coleção 'anunciantes'
        const anunciantesRef = collection(db, 'anunciantes');

        try {
            // Adiciona um novo documento na coleção
            const docRef = await addDoc(anunciantesRef, data); // 'data' é o objeto com os dados do novo anunciante

            console.log("Documento adicionado com ID: ", docRef.id);
            return docRef.id; // Retorna o ID do documento adicionado
        } catch (e) {
            console.error("Erro ao adicionar documento: ", e);
            throw e;
        }
    }
};
