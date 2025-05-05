// Importar os módulos necessários do Firebase SDK
import { initializeApp } from "https://www.gstatic.com/firebasejs/11.4.0/firebase-app.js";
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
import { getFirestore, collection, getDocs, addDoc, query, where, } from "https://www.gstatic.com/firebasejs/11.4.0/firebase-firestore.js";
=======
import { getFirestore, collection, getDocs, addDoc, doc, query, where, onSnapshot } from "https://www.gstatic.com/firebasejs/11.4.0/firebase-firestore.js";
>>>>>>> Stashed changes
=======
import { getFirestore, collection, getDocs, addDoc, doc, query, where, onSnapshot } from "https://www.gstatic.com/firebasejs/11.4.0/firebase-firestore.js";
>>>>>>> Stashed changes
=======
import { getFirestore, collection, getDocs, addDoc, doc, query, where, onSnapshot } from "https://www.gstatic.com/firebasejs/11.4.0/firebase-firestore.js";
>>>>>>> Stashed changes
import { getAuth, setPersistence, browserLocalPersistence, signInWithEmailAndPassword, createUserWithEmailAndPassword, sendPasswordResetEmail, signOut, onAuthStateChanged } from "https://www.gstatic.com/firebasejs/11.4.0/firebase-auth.js";
import { getStorage, ref, uploadBytes, getDownloadURL } from "https://www.gstatic.com/firebasejs/11.4.0/firebase-storage.js";
// Inicializar o Firebase com a configuração fornecida
window.firebaseService = {
    app: null,
    db: null,
    auth: null,
    dotnetHelper: null,
    storage: null,

    initializeApp: function (config) {
        this.app = initializeApp(config);  // Inicializa o Firebase com a configuração
        this.db = getFirestore(this.app);  // Obtém a instância do Firestore
        this.auth = getAuth(this.app); // Obtém a instância de autenticação
        this.storage = getStorage(this.app); // Adiciona o Firebase Storage aqui
        // Verifica se há usuário logado ao carregar o app
        onAuthStateChanged(this.auth, (user) => {
            if (user && this.dotnetHelper) {
                this.dotnetHelper.invokeMethodAsync("SetUserLogged", user.email);
            } else if (this.dotnetHelper) {
                this.dotnetHelper.invokeMethodAsync("SetUserLoggedOut");
            }
        });

        return this.db;  // Retorna a instância do Firestore
    },

    setDotnetHelper: function (helper) {
        this.dotnetHelper = helper;
    },

    getProdutos: async function () {
    const db = getFirestore(); // Usa o Firestore já inicializado

    const productsSnap = await getDocs(query(collection(db, "products"), where("active", "==", true)));
    const produtos = [];

    for (const productDoc of productsSnap.docs) {
        const data = productDoc.data();
        const product = {
            id: productDoc.id,
            name: data.name,
            active: data.active,
            precos: []
        };

        const pricesSnap = await getDocs(collection(productDoc.ref, "prices"));
        pricesSnap.forEach(priceDoc => {
            const priceData = priceDoc.data();
            product.precos.push({
                id: priceDoc.id,
                unit_amount: priceData.unit_amount,
                description: priceData.description,
                currency: priceData.currency
            });
        });

        produtos.push(product);
    }

    return produtos;
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
            await setPersistence(this.auth, browserLocalPersistence);
            const userCredential = await createUserWithEmailAndPassword(this.auth, email, password);
            return userCredential.user;
        } catch (error) {
            console.error("Erro ao cadastrar usuário:", error.message);
            return null;
        }
    },

    login: async function (email, password) {
        try {
            await setPersistence(this.auth, browserLocalPersistence);
            const userCredential = await signInWithEmailAndPassword(this.auth, email, password);
            return userCredential.user;
        } catch (error) {
            console.error("Erro ao fazer login: ", error);
            return null;
        }
    },

    getCurrentUserUid: async function () {
        const user = getAuth().currentUser;
        if (user) {
            return user.uid;
        } else {
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
    },

<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
    uploadImageFromInput: async function (element, path) {
        const file = element.files[0];
        if (!file) throw new Error("Nenhum arquivo selecionado");

        const storageRef = ref(this.storage, path);
        try {
            const snapshot = await uploadBytes(storageRef, file);
            const url = await getDownloadURL(snapshot.ref);
            return url;
        } catch (error) {
            console.error("Erro ao fazer upload da imagem:", error);
            throw error;
        }
    },

    getCurrentUserId: () => {
        //console.log("getCurrentUserId", this.auth)
        const auth = getAuth();
        const user = auth.currentUser;
        return user ? user.uid : null;
    },

=======
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
    // Adiciona uma sessão de checkout
    addCheckoutSession: async function (uid, sessionData) {
        const db = getFirestore();

        // Caminho: /customers/{uid}/checkout_sessions
        const checkoutSessionsRef = collection(doc(collection(db, "customers"), uid), "checkout_sessions");

        const docRef = await addDoc(checkoutSessionsRef, sessionData);
        return {
            id: docRef.id,
            path: docRef.path
        };
    },

    listenToCheckoutSession: function (uid, docId, dotNetHelper) {
        const db = getFirestore();
        const checkoutSessionRef = doc(collection(doc(collection(db, "customers"), uid), "checkout_sessions"), docId);

        return onSnapshot(checkoutSessionRef, (docSnapshot) => {
            if (docSnapshot.exists()) {
                dotNetHelper.invokeMethodAsync("OnCheckoutSessionChanged", docSnapshot.data());
            }
        });
    },

    getActiveSubscriptionStatus: async function (uid) {
        const db = getFirestore();
        const subscriptionsSnapshot = await getDocs(query(
            collection(doc(collection(db, "customers"), uid), "subscriptions"),
            where("status", "in", ["trialing", "active"])
        ));

        return !subscriptionsSnapshot.empty;
    },
<<<<<<< Updated upstream
<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
};


