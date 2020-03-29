from concurrent import futures
import logging

import grpc

import dateparser_pb2
import dateparser_pb2_grpc

#TODO: Error handling
#TODO: Add logging
class DateParser(dateparser_pb2_grpc.DateParserServicer):

    def ParseDate(self, request, context):
        return dateparser_pb2.ParseDateReply(ParsedDate='Hello, %s!' % request.NaturalDate)


def serve():
    server = grpc.server(futures.ThreadPoolExecutor(max_workers=10))
    dateparser_pb2_grpc.add_DateParserServicer_to_server(DateParser(), server)
    server.add_insecure_port('[::]:80')
    server.start()
    server.wait_for_termination()


if __name__ == '__main__':
    logging.basicConfig()
    serve()
