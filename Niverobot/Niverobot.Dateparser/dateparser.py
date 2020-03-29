from concurrent import futures
import logging

import grpc

import dateparser_pb2
import dateparser_pb2_grpc

# TODO: error handling


class DataParser(dateparser_pb2_grpc.DateparserServicer):

    def ParseDate(self, request, context):
        return dateparser_pb2.NaturalDateResponse(dateTime='Hello, %s!' % request.naturalDate)


def serve():
    server = grpc.server(futures.ThreadPoolExecutor(max_workers=10))
    dateparser_pb2_grpc.add_DateparserServicer_to_server(DataParser(), server)
    server.add_insecure_port('[::]:50051')
    server.start()
    server.wait_for_termination()


if __name__ == '__main__':
    logging.basicConfig()
    serve()
