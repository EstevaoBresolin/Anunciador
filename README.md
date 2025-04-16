# Conecte Membros

**Conecte Membros** é uma plataforma criada para conectar membros de igrejas a diversos serviços oferecidos por profissionais da comunidade.  
Anunciantes como **dentistas, barbeiros e consultores financeiros** podem divulgar seus serviços mediante uma **assinatura mensal**, enquanto os membros da igreja têm **acesso gratuito** para buscar e encontrar os serviços que precisam.

## 🚀 Funcionalidades

- ✅ **Acesso gratuito para membros da igreja**: Qualquer membro pode buscar e visualizar serviços sem custo.  
- 💰 **Plano único para anunciantes**: Profissionais podem divulgar seus serviços mediante uma mensalidade fixa.  
- 🔍 **Busca e categorização**: Os serviços estão organizados em categorias como _"Saúde e Beleza"_, _"Construção"_, _"Financeiro"_, entre outros.  
- 📌 **Página geral e por categorias**: Os anunciantes aparecem em uma página geral e também dentro de suas categorias específicas.  
- 🔎 **Busca por nome, título e especialidade**: Facilidade para encontrar serviços específicos.  
- 🖼 **Hospedagem gratuita de imagens**: As imagens dos anunciantes são armazenadas no **Imgur**.  

## 🛠 Tecnologias Utilizadas

- ⚡ **Blazor WebAssembly** - Aplicativo frontend  
- 🎨 **MudBlazor** - Biblioteca de componentes para UI  
- 🔥 **Firebase Firestore** - Banco de dados para armazenamento dos anunciantes  
- 🌍 **GitHub Pages** - Hospedagem gratuita do aplicativo  
- 🖼 **Imgur** - Hospedagem gratuita para imagens dos anunciantes  

## 📖 Como Usar

1️⃣ **Acesse a plataforma** através do link 👉 [Conecte Membros](https://estevaobresolin.github.io/Anunciador/)  
2️⃣ **Navegue entre as categorias** ou use a barra de pesquisa para encontrar um serviço.  
3️⃣ **Anunciantes interessados** podem cadastrar seus serviços através da opção **"Cadastrar Anunciante"**.  

---

## 🔄 TROCAR BASE AO COMPILAR EM DEBUG

```html
<base href="/" />
<base href="https://EstevaoBresolin.github.io/Anunciador/" />

-------- Publicar manualmente na vercel --------

dotnet publish -c Release


-------- Publicar manualmente na pasta docs --------


dotnet publish -c:Release -o docs --nologo



--------- LIMITES DO PLANO GRATUITO FIREBASE -----------



- 1GB de armazenamento

- 10GB de transferência por mês

- 20K gravações por dia

- 50k leituras por dia

- 20k exclusões por dia



--------- Imagens Hospedadas ---------



https://estevaobresolin.imgur.com/all
