const functions = require('firebase-functions/v1');
const admin = require('firebase-admin');
const { MailerSend, EmailParams, Sender, Recipient } = require('mailersend');

admin.initializeApp();

exports.stripeWebhookCheckout = functions.runWith({
  secrets: ["STRIPE_SECRET_KEY", "STRIPE_WEBHOOK_SECRET", "MAILERSEND_API_KEY"]
}).https.onRequest(async (req, res) => {
  const stripeKey = process.env.STRIPE_SECRET_KEY || 'dummy';
  const stripe = require('stripe')(stripeKey);
  const webhookSecret = process.env.STRIPE_WEBHOOK_SECRET;

  const mailersend = new MailerSend({
    apiKey: process.env.MAILERSEND_API_KEY || 'dummy',
  });

  const sig = req.headers['stripe-signature'];

  let event;

  try {
    event = stripe.webhooks.constructEvent(req.rawBody, sig, webhookSecret);
  } catch (err) {
    console.error('⚠️  Webhook signature verification failed.', err.message);
    return res.status(400).send(`Webhook Error: ${err.message}`);
  }

  if (event.type === 'checkout.session.completed') {
    const session = event.data.object;
    // client_reference_id é injetado pelo app Desktop na URL do Stripe Checkout!
    let uid = session.client_reference_id;
    const customerEmail = session.customer_details?.email;
    const customerId = session.customer;

    // Fallback: Se o usuário assinou pelo site ou perdeu o reference_id, busca pelo E-mail!
    if (!uid && customerEmail) {
      try {
        const userRecord = await admin.auth().getUserByEmail(customerEmail);
        uid = userRecord.uid;
        console.log(`Fallback: Encontrado UID ${uid} via email do Stripe (${customerEmail}).`);
      } catch (err) {
        console.warn(`Fallback Falhou: Email ${customerEmail} não encontrado no Firebase Auth.`);
      }
    }

    if (uid) {
      try {
        // 1. Desbloqueia a conta gravando no Firestore e salva o ID do cliente
        await admin.firestore().collection('users').doc(uid).set(
          { 
            plan: 'pro',
            stripeCustomerId: customerId
          },
          { merge: true }
        );
        console.log(`✅  User ${uid} successfully upgraded to PRO. Customer: ${customerId}`);
        
        // 2. Dispara E-mail de Boas-vindas pelo MailerSend
        if (customerEmail && process.env.MAILERSEND_API_KEY) {
          const sentFrom = new Sender("suporte@foreigntechnologies.com.br", "Desktop Command Center");
          const recipients = [new Recipient(customerEmail, "Assinante PRO")];
          
          const emailParams = new EmailParams()
            .setFrom(sentFrom)
            .setTo(recipients)
            .setSubject("Bem-vindo ao Desktop Command Center PRO! 🚀")
            .setHtml(`
              <h2>Olá!</h2>
              <p>Sua assinatura foi processada com sucesso. Muito obrigado por apoiar o desenvolvimento do DCC!</p>
              <p>O seu plano <strong>PRO</strong> já está ativado. Basta reiniciar ou acessar o aplicativo que as IAs e funcionalidades premium já estarão desbloqueadas e prontas para uso.</p>
              <br/>
              <p>Qualquer dúvida, responda a este e-mail. Estarei à disposição!</p>
              <br/>
              <p>Equipe Foreign Technologies</p>
            `)
            .setText("Sua assinatura foi processada. Seu plano PRO já está ativo. Obrigado!");

          try {
            await mailersend.email.send(emailParams);
            console.log(`📧 E-mail de boas-vindas enviado para ${customerEmail}`);
          } catch (emailErr) {
            console.error(`⚠️ Falha ao enviar email pelo MailerSend para ${customerEmail}, mas a ativação do plano no Firestore foi concluída:`, emailErr);
            if (emailErr.body) {
              console.error('Detalhes do erro do MailerSend:', JSON.stringify(emailErr.body, null, 2));
            }
          }
        } else {
            console.log(`⚠️ Email não enviado: customerEmail ausente ou chave do Mailersend não configurada.`);
        }
      } catch (error) {
        console.error('Erro ao atualizar usuário ou enviar email:', error);
        return res.status(500).send('Erro interno');
      }
    } else {
      console.warn('⚠️  checkout.session.completed sem client_reference_id!');
    }
  } 
  
  // Tratamento para quando a assinatura é atualizada, pausada, cancelada ou expira, ou cliente deletado
  if (event.type === 'customer.subscription.deleted' || event.type === 'customer.subscription.updated' || event.type === 'customer.subscription.paused' || event.type === 'customer.deleted') {
    const obj = event.data.object;
    
    let customerId;
    let status;
    
    if (event.type === 'customer.deleted') {
      customerId = obj.id;
      status = 'deleted'; // Força status deletado para rebaixar para free
    } else {
      customerId = obj.customer;
      status = obj.status; // 'active', 'paused', 'canceled', 'trialing', etc.
    }

    if (customerId) {
      try {
        // Busca qual usuário do nosso banco tem esse customerId do Stripe
        const usersSnapshot = await admin.firestore().collection('users')
          .where('stripeCustomerId', '==', customerId)
          .limit(1)
          .get();

        if (!usersSnapshot.empty) {
          const userDoc = usersSnapshot.docs[0];
          
          if (event.type === 'customer.subscription.deleted' || event.type === 'customer.deleted' || (status !== 'active' && status !== 'trialing' && status !== 'paused')) {
            // Rebaixa o usuário para o plano grátis
            await userDoc.ref.set({ plan: 'free' }, { merge: true });
            console.log(`🚫 Assinatura/Cliente ${status}! Usuário ${userDoc.id} rebaixado para free.`);
          } else if (status === 'paused') {
            await userDoc.ref.set({ plan: 'paused' }, { merge: true });
            console.log(`⏸ Assinatura ${status}! Usuário ${userDoc.id} pausado.`);
          } else if (status === 'active' || status === 'trialing') {
            // Garante que retorne/mantenha o pro caso a assinatura esteja ativa ou em trial
            await userDoc.ref.set({ plan: 'pro' }, { merge: true });
            console.log(`✅ Assinatura ${status}! Usuário ${userDoc.id} atualizado/restaurado para pro.`);
          }
        } else {
          console.log(`⚠️ Evento de assinatura recebido, mas não achamos o usuário com o customerId ${customerId}.`);
        }
      } catch (error) {
        console.error('Erro ao gerenciar status da assinatura do usuário:', error);
        return res.status(500).send('Erro interno');
      }
    }
  }

  res.json({ received: true });
});
