// Importar os módulos necessários do Firebase SDK
import { initializeApp } from "https://www.gstatic.com/firebasejs/11.4.0/firebase-app.js";
import { getFirestore, collection, getDoc, getDocs, addDoc, updateDoc, deleteDoc, doc, query, where, onSnapshot } from "https://www.gstatic.com/firebasejs/11.4.0/firebase-firestore.js";
import { getAuth, setPersistence, browserLocalPersistence, signInWithEmailAndPassword, createUserWithEmailAndPassword, sendPasswordResetEmail, signOut, onAuthStateChanged } from "https://www.gstatic.com/firebasejs/11.4.0/firebase-auth.js";
import { getStorage, ref, uploadBytes, getDownloadURL } from "https://www.gstatic.com/firebasejs/11.4.0/firebase-storage.js";
import { getFunctions, httpsCallable } from "https://www.gstatic.com/firebasejs/11.4.0/firebase-functions.js";

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
                this.dotnetHelper.invokeMethodAsync("SetUserLogged", user.email, user.uid);
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
        //const anunciantesRef = collection(db, 'anunciantes');  // Referência à coleção
        //const querySnapshot = await getDocs(anunciantesRef);  // Obtém os documentos da coleção


        const anunciantesRef = collection(db, 'anunciantes');
        const ativosQuery = query(anunciantesRef, where('ativoInativo', '==', true)); // ou 'ativo', se for o nome do campo
        const querySnapshot = await getDocs(ativosQuery);

        // Mapeia os documentos para um formato legível
        return querySnapshot.docs.map(doc => ({
            id: doc.id,
            ...doc.data()
        }));
        //return querySnapshot.docs.map(doc => doc.data());
    },

    getAnunciosUsuario: async function (UidUsuario) {
        const db = getFirestore();

        const anunciantesRef = collection(db, 'anunciantes');
        const ativosQuery = query(anunciantesRef, where('uidUsuario', '==', UidUsuario)); // ou 'ativo', se for o nome do campo
        const querySnapshot = await getDocs(ativosQuery);

        // Mapeia os documentos para um formato legível
        return querySnapshot.docs.map(doc => ({
            idFirestore: doc.id,
            ...doc.data()
        }));
        //return querySnapshot.docs.map(doc => doc.data());
    },

    getAnuncioPorId: async function (id) {
        const db = getFirestore();

        const anuncioRef = doc(db, "anunciantes", id); // Acessa diretamente o doc pelo ID
        const anuncioSnap = await getDoc(anuncioRef);

        if (anuncioSnap.exists()) {
            return {
                id: anuncioSnap.id,
                ...anuncioSnap.data()
            };
        } else {
            return null; // Documento não encontrado
        }
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

    updateAnuncio: async function (id, novosDados) {
        const db = getFirestore();

        if (!id || !novosDados) {
            throw new Error('ID ou dados inválidos');
        }

        const docRef = doc(db, 'anunciantes', id);
        await updateDoc(docRef, novosDados);
    },

    deleteAnuncio: async function (id) {
        const db = getFirestore();

        if (!id) {
            throw new Error('ID inválido');
        }

        const docRef = doc(db, 'anunciantes', id);
        await deleteDoc(docRef);
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

    createLinkPortalCliente: async function (returnUrl) {
        const functions = getFunctions(this.app, "southamerica-east1"); // região
        const createPortalLink = httpsCallable(functions, "ext-firestore-stripe-payments-createPortalLink");

        const { data } = await createPortalLink({
            returnUrl: returnUrl,
            locale: "auto",
            configuration: null, // ou remova se não quiser
        })

        window.location.assign(data.url);
    },

    getActiveSubscriptionStatus: async function (uid) {
        const db = getFirestore();
        const subscriptionsQuery = query(
            collection(doc(collection(db, "customers"), uid), "subscriptions"),
            where("status", "in", ["trialing", "active"])
        );

        const subscriptionsSnapshot = await getDocs(subscriptionsQuery);

        if (subscriptionsSnapshot.empty) return null;

        const sub = subscriptionsSnapshot.docs[0].data();
        return {
            ativa: true,
            status: sub.status,
            tipo: sub.items?.[0]?.price?.nickname ?? "Plano desconhecido",
            dataExpiracao: sub.current_period_end?.seconds
                ? new Date(sub.current_period_end.seconds * 1000).toISOString().split("T")[0]
                : "Data desconhecida"
        };
    },

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


};


