from pydantic import BaseModel
from typing import List, Literal


class Item(BaseModel):
    name: str
    price: float


class PaymentRequest(BaseModel):
    items: List[Item]
    total_amount: float
    payment_method: Literal["credit_card", "pix"]
