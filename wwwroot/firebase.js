// Importar os módulos necessários do Firebase SDK
import { initializeApp } from "https://www.gstatic.com/firebasejs/11.4.0/firebase-app.js";
import { getFirestore, collection, getDocs, addDoc } from "https://www.gstatic.com/firebasejs/11.4.0/firebase-firestore.js";
import { getAuth, signInWithEmailAndPassword, createUserWithEmailAndPassword, sendPasswordResetEmail , signOut } from "https://www.gstatic.com/firebasejs/11.4.0/firebase-auth.js";
// Inicializar o Firebase com a configuração fornecida
window.firebaseService = {
    app: null,
    db: null,
    auth: null,

    initializeApp: function (config) {
        this.app = initializeApp(config);  // Inicializa o Firebase com a configuração
        this.db = getFirestore(this.app);  // Obtém a instância do Firestore
        this.auth = getAuth(this.app); // Obtém a instância de autenticação
        return this.db;  // Retorna a instância do Firestore
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
    },

    registerUser: async function (email, password) {
        try {
            const userCredential = await createUserWithEmailAndPassword(this.auth, email, password);
            console.log("Usuário cadastrado:", userCredential.user);
            return userCredential.user;
        } catch (error) {
            console.error("Erro ao cadastrar usuário:", error.message);
            return null;
        }
    },

    login: async function (email, password) {
        if (!this.auth) {
            throw new Error('Auth não foi inicializado corretamente');
        }
        try {
            const userCredential = await signInWithEmailAndPassword(this.auth, email, password);
            return userCredential.user;
        } catch (error) {
            console.error("Erro ao fazer login: ", error);
            return null;
        }
    },

    logout: async function () {
        if (!this.auth) {
            throw new Error('Auth não foi inicializado corretamente');
        }
        await signOut(this.auth);
    },

    recuperarSenha: async function (email) {
        try {
            await sendPasswordResetEmail(this.auth, email);
            console.log("E-mail de redefinição enviado.");
        } catch (error) {
            console.error("Erro ao enviar e-mail de redefinição:", error);
        }
    }
};
