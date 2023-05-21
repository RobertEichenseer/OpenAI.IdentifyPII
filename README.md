# Using Azure OpenAI to identify Personal Identifiable Information (PII)


## Intention

Removing PII from data is getting more and more important when further insights from data sets need to be created without disrespecting local laws and/or company policies.

The example in this repo is intended to serve as a quick start for own developments or experiments with LLMs to remove PII from provided information.

## Content

The repo contains:

- [Step-by-step guidance](src/CreateEnv/CreateEnv.azcli) to create an Azure OpenAI Cognitive Services instance with a deployment of the gpt35-turbo model
- [Simplified sample prompt](src/Prompts/PII.txt) using few-shot learning to identify PII in provided text
- [Simplified call to the Azure OpenAI Cognitive Service](src/SimpleClient.curl/SimpleClient.azcli) using curl
- Simplified C# client using HttpClient (not the Azure OpenAI SDK) to call the created Azure OpenAI Cognitive Service
  - [Polyglot notebook](src/SimpleClient.Net.HttpClient/SimpleClient.ipynb) with further explanations
  - [C# console application](src/SimpleClient.Net.HttpClient/README.md) using HttpClient

## Interface

In Azure OpenAI there are two different options for interacting LLM models:

- Chat Completion API.
- Completion API with Chat Markup Language (ChatML).

The example in this repo interacts with the  Completion API. The deployed model (gpt35-turbo) provides both API versions (Chat Completion & Completion). "Older" LLM models just providing the completion API can be used as well.

## Prompt

The simplified prompt uses [few-shot learning](https://learn.microsoft.com/en-us/azure/cognitive-services/openai/concepts/advanced-prompt-engineering?pivots=programming-language-chat-completions) to provide a set of training examples as part of the prompt to give context to the model.

The following simplified prompt is used where the model completes 'Ashley has bought a news paper at the airport' with removed PII:

```md
You are a tool which extracts personal identifiable information (PII) from data provided to you.
If you are unsure if there is PII in the data answer with 'The provided dataset needs checking'.
Do not invent additional information.
Do not include potential PII information in questions you add to your response.

Example 1:
Input: Susanne is a power user and works daily 8 hours with the system
Response: xx is a power user and works daily 8 hours with the system

Example 2:
Input: Charles has used Outlook 15 minutes yesterday
Response: xx has used Outlook 15 minutes yesterday

Example 3:
Input: Robert is driving a car at high speed
Response: xx is driving a car at high speed

Example 4:
Input: Jon has an invitation to a restaurant
Response: xx has an invitation to a restaurant

Example 5:
Input: Olivia is running fast during the competition
Response: xx is running fast during the competition
  
'Ashley has bought a news paper at the airport'
```
