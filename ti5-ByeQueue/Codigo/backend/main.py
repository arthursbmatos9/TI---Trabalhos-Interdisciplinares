from fastapi import APIRouter, HTTPException
from pydantic import BaseModel
import stripe
import os
from dotenv import load_dotenv

load_dotenv()

stripe.api_key = os.getenv("STRIPE_SECRET_KEY")

app = APIRouter()

class PaymentRequest(BaseModel):
    total_amount: float
    items: list

@app.post("/create-payment-intent")
def create_payment_intent(payment_request: PaymentRequest):
    try:
        # Stripe espera valor em centavos
        amount_in_cents = int(payment_request.total_amount * 100)

        intent = stripe.PaymentIntent.create(
            amount=amount_in_cents,
            currency="brl",
            payment_method_types=["card"],
        )

        return {"client_secret": intent.client_secret}

    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))
