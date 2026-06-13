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
    const uid = session.client_reference_id;
    const customerEmail = session.customer_details?.email;
    const customerId = session.customer;

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
              <p>O seu plano <strong>PRO Enterprise</strong> já está ativado. Basta reiniciar ou acessar o aplicativo que as IAs e funcionalidades premium já estarão desbloqueadas e prontas para uso.</p>
              <br/>
              <p>Qualquer dúvida, responda a este e-mail. Estarei à disposição!</p>
              <br/>
              <p>Equipe Foreign Technologies</p>
            `)
            .setText("Sua assinatura foi processada. Seu plano PRO já está ativo. Obrigado!");

          await mailersend.email.send(emailParams);
          console.log(`📧 E-mail de boas-vindas enviado para ${customerEmail}`);
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
  
  // Tratamento para quando a assinatura é cancelada ou expira por falta de pagamento
  if (event.type === 'customer.subscription.deleted') {
    const subscription = event.data.object;
    const customerId = subscription.customer;

    if (customerId) {
      try {
        // Busca qual usuário do nosso banco tem esse customerId do Stripe
        const usersSnapshot = await admin.firestore().collection('users')
          .where('stripeCustomerId', '==', customerId)
          .limit(1)
          .get();

        if (!usersSnapshot.empty) {
          const userDoc = usersSnapshot.docs[0];
          // Rebaixa o usuário para o plano grátis
          await userDoc.ref.set({ plan: 'free' }, { merge: true });
          console.log(`🚫 Assinatura cancelada! Usuário ${userDoc.id} rebaixado para free.`);
        } else {
          console.log(`⚠️ Assinatura cancelada, mas não achamos o usuário com o customerId ${customerId}.`);
        }
      } catch (error) {
        console.error('Erro ao cancelar assinatura do usuário:', error);
        return res.status(500).send('Erro interno');
      }
    }
  }

  res.json({ received: true });
});
