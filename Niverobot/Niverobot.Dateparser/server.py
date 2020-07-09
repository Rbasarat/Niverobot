from concurrent import futures
import logging
import grpc
from dateparser.search import search_dates
from google.protobuf.timestamp_pb2 import Timestamp
import datetime

import dateparser_pb2
import dateparser_pb2_grpc

# TODO: Error handling
# TODO: Add logging
# https://github.com/scrapinghub/dateparser#features


class DateParser(dateparser_pb2_grpc.DateParserServicer):

    def ParseDate(self, request, context):
        date = search_dates(request.NaturalDate, settings={
                            'RETURN_AS_TIMEZONE_AWARE': True, 'RETURN_TIME_AS_PERIOD': True, 'PREFER_DATES_FROM': 'future', 'RELATIVE_BASE': datetime.datetime.utcnow(), 'DATE_ORDER': 'DMY'})
        return self.CreateResponse(date)

    def CreateResponse(self, date):
        timestamp = Timestamp()
        if date is not None:
            timestamp.FromDatetime((date[0][1]))
            return dateparser_pb2.ParseDateReply(ParsedDate=timestamp, Offset=date[0][1].utcoffset().total_seconds(), Date=date[0][0])
        else:
            return dateparser_pb2.ParseDateReply(ParsedDate=timestamp, Offset=0, Date="Error")


def serve():
    server = grpc.server(futures.ThreadPoolExecutor(max_workers=10))
    dateparser_pb2_grpc.add_DateParserServicer_to_server(DateParser(), server)
    server.add_insecure_port('[::]:50051')
    server.start()
    print("server running")
    server.wait_for_termination()


if __name__ == '__main__':
    logging.basicConfig()
    serve()