#!/bin/bash
systemctl daemon-reload
if [ -f "/etc/systemd/system/ocbt.service" ]; then
    service ocbt stop
fi
