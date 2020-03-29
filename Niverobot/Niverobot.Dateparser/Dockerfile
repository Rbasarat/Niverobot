FROM python:3.8 as base

WORKDIR /usr/src/app

COPY Niverobot.Dateparser .
Copy Niverobot.Protos ./protos

RUN python -m pip install --upgrade pip
RUN python -m pip install grpcio
RUN python -m pip install grpcio-tools
RUN python -m pip install dateparser
RUN python -m grpc_tools.protoc -I./protos --python_out=. --grpc_python_out=. ./protos/dateparser.proto

CMD [ "python", "./server.py" ]